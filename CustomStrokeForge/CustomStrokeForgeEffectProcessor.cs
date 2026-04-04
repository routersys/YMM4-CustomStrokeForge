using System.Numerics;
using Vortice.Direct2D1;
using Vortice.DXGI;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;

namespace CustomStrokeForge
{
    internal sealed class CustomStrokeForgeEffectProcessor : IVideoEffectProcessor, IDisposable
    {
        private readonly CustomStrokeForgeEffect _item;
        private readonly IGraphicsDevicesAndContext _devices;

        private ID2D1Image? _input;
        private ID2D1Bitmap1? _bufferA;
        private ID2D1Bitmap1? _bufferB;
        private ID2D1CommandList? _commandList;
        private StrokeCustomEffect? _shader;

        public ID2D1Image Output => _commandList ?? throw new NullReferenceException();

        public CustomStrokeForgeEffectProcessor(
            IGraphicsDevicesAndContext devices,
            CustomStrokeForgeEffect item)
        {
            _devices = devices;
            _item = item;
            _shader = new StrokeCustomEffect(devices);
        }

        public void SetInput(ID2D1Image? input) => _input = input;
        public void ClearInput() => _input = null;

        public DrawDescription Update(EffectDescription effectDescription)
        {
            var frame = effectDescription.ItemPosition.Frame;
            var length = effectDescription.ItemDuration.Frame;
            var fps = effectDescription.FPS;

            float width = (float)_item.StrokeWidth.GetValue(frame, length, fps);
            float soft = (float)_item.Softness.GetValue(frame, length, fps);
            float opacity = (float)(_item.StrokeOpacity.GetValue(frame, length, fps) / 100.0);
            var color = _item.StrokeColor;

            var dc = _devices.DeviceContext;

            _commandList?.Dispose();
            _commandList = dc.CreateCommandList();

            var originalTarget = dc.Target;
            var originalTransform = dc.Transform;

            dc.Target = _commandList;
            dc.BeginDraw();
            dc.Clear(null);

            if (_input != null && width > 0.0f && opacity > 0.0f && _shader != null && _shader.IsEnabled)
            {
                var bounds = dc.GetImageLocalBounds(_input);

                if (bounds.Right > bounds.Left && bounds.Bottom > bounds.Top)
                {
                    float padding = width + soft + 8.0f;
                    int w = (int)Math.Ceiling(bounds.Right - bounds.Left + padding * 2);
                    int h = (int)Math.Ceiling(bounds.Bottom - bounds.Top + padding * 2);

                    EnsureBuffers(dc, w, h);

                    dc.Transform = Matrix3x2.Identity;

                    var bufferOrigin = new Vector2(bounds.Left - padding, bounds.Top - padding);
                    var drawOffset = new Vector2(padding - bounds.Left, padding - bounds.Top);
                    var transparent = new Vortice.Mathematics.Color4(0, 0, 0, 0);

                    dc.Target = _bufferA;
                    dc.Clear(transparent);
                    dc.DrawImage(_input, drawOffset);

                    _shader.TextureSize = new Vector2(w, h);
                    _shader.Threshold = 0.01f;

                    _shader.Mode = 0;
                    _shader.SetInput(0, _bufferA!, true);
                    dc.Target = _bufferB;
                    dc.Clear(transparent);
                    using (var output = _shader.Output)
                        dc.DrawImage(output);

                    _shader.Mode = 1;
                    var src = _bufferB!;
                    var dst = _bufferA!;
                    int maxDim = Math.Max(w, h);
                    int steps = (int)Math.Ceiling(Math.Log2(maxDim));

                    for (int i = 0; i < steps; i++)
                    {
                        float step = (float)Math.Pow(2, steps - 1 - i);
                        if (step < 0.9f) break;

                        dc.Target = dst;
                        dc.Clear(transparent);
                        _shader.StepWidth = step;
                        _shader.SetInput(0, src, true);
                        using (var output = _shader.Output)
                            dc.DrawImage(output);

                        (src, dst) = (dst, src);
                    }

                    for (int k = 0; k < 2; k++)
                    {
                        dc.Target = dst;
                        dc.Clear(transparent);
                        _shader.StepWidth = 1.0f;
                        _shader.SetInput(0, src, true);
                        using (var output = _shader.Output)
                            dc.DrawImage(output);

                        (src, dst) = (dst, src);
                    }

                    _shader.Mode = 2;
                    _shader.SetInput(0, src, true);
                    _shader.OutlineColor = new Vector4(
                        color.R / 255f,
                        color.G / 255f,
                        color.B / 255f,
                        (color.A / 255f) * opacity);
                    _shader.BorderWidth = width;
                    _shader.Softness = soft;
                    _shader.StyleMode = (float)(int)_item.Style;
                    _shader.DashLen = (float)_item.DashLength.GetValue(frame, length, fps);
                    _shader.GapLen = (float)_item.GapLength.GetValue(frame, length, fps);
                    _shader.ShortDashLen = (float)_item.ShortDashLength.GetValue(frame, length, fps);
                    _shader.WaveAmp = (float)_item.WaveAmplitude.GetValue(frame, length, fps);
                    _shader.WaveFreq = (float)_item.WaveFrequency.GetValue(frame, length, fps);
                    _shader.PhaseOffset = (float)_item.PhaseOffset.GetValue(frame, length, fps);
                    _shader.DoubleGap = (float)_item.DoubleGap.GetValue(frame, length, fps);
                    _shader.PositionMode = (float)(int)_item.Position;

                    dc.Target = _commandList;
                    dc.Transform = originalTransform;

                    using (var output = _shader.Output)
                        dc.DrawImage(output, bufferOrigin);

                    dc.DrawImage(_input);
                }
                else
                {
                    dc.Transform = originalTransform;
                    dc.DrawImage(_input);
                }
            }
            else if (_input != null)
            {
                dc.Transform = originalTransform;
                dc.DrawImage(_input);
            }

            dc.Transform = originalTransform;
            dc.EndDraw();
            dc.Target = originalTarget;
            _commandList!.Close();

            return effectDescription.DrawDescription;
        }

        private void EnsureBuffers(ID2D1DeviceContext dc, int width, int height)
        {
            var fmt = new Vortice.DCommon.PixelFormat(
                Format.R32G32B32A32_Float,
                Vortice.DCommon.AlphaMode.Premultiplied);

            if (_bufferA == null
                || _bufferA.PixelSize.Width != width
                || _bufferA.PixelSize.Height != height)
            {
                _bufferA?.Dispose();
                _bufferA = dc.CreateBitmap(
                    new Vortice.Mathematics.SizeI(width, height),
                    IntPtr.Zero, 0,
                    new BitmapProperties1(fmt, 96, 96, BitmapOptions.Target));
            }

            if (_bufferB == null
                || _bufferB.PixelSize.Width != width
                || _bufferB.PixelSize.Height != height)
            {
                _bufferB?.Dispose();
                _bufferB = dc.CreateBitmap(
                    new Vortice.Mathematics.SizeI(width, height),
                    IntPtr.Zero, 0,
                    new BitmapProperties1(fmt, 96, 96, BitmapOptions.Target));
            }
        }

        public void Dispose()
        {
            _bufferA?.Dispose();
            _bufferB?.Dispose();
            _commandList?.Dispose();
            _shader?.SetInput(0, null, true);
            _shader?.Dispose();
        }
    }
}
