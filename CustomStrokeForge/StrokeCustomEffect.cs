using System.Numerics;
using System.Runtime.InteropServices;
using Vortice;
using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;

namespace CustomStrokeForge
{
    public sealed class StrokeCustomEffect : D2D1CustomShaderEffectBase
    {
        public StrokeCustomEffect(IGraphicsDevicesAndContext devices)
            : base(Create<EffectImpl>(devices)) { }

        public Vector2 TextureSize { set => SetValue((int)EffectImpl.Props.TextureSize, value); }
        public float StepWidth { set => SetValue((int)EffectImpl.Props.StepWidth, value); }
        public int Mode { set => SetValue((int)EffectImpl.Props.Mode, value); }
        public Vector4 OutlineColor { set => SetValue((int)EffectImpl.Props.OutlineColor, value); }
        public float BorderWidth { set => SetValue((int)EffectImpl.Props.BorderWidth, value); }
        public float Softness { set => SetValue((int)EffectImpl.Props.Softness, value); }
        public float Threshold { set => SetValue((int)EffectImpl.Props.Threshold, value); }
        public float StyleMode { set => SetValue((int)EffectImpl.Props.StyleMode, value); }
        public float DashLen { set => SetValue((int)EffectImpl.Props.DashLen, value); }
        public float GapLen { set => SetValue((int)EffectImpl.Props.GapLen, value); }
        public float ShortDashLen { set => SetValue((int)EffectImpl.Props.ShortDashLen, value); }
        public float WaveAmp { set => SetValue((int)EffectImpl.Props.WaveAmp, value); }
        public float WaveFreq { set => SetValue((int)EffectImpl.Props.WaveFreq, value); }
        public float PhaseOffset { set => SetValue((int)EffectImpl.Props.PhaseOffset, value); }
        public float DoubleGap { set => SetValue((int)EffectImpl.Props.DoubleGap, value); }
        public float PositionMode { set => SetValue((int)EffectImpl.Props.PositionMode, value); }

        internal void ClearInput() => SetInput(0, null, true);

        [CustomEffect(1)]
        internal sealed class EffectImpl : D2D1CustomShaderEffectImplBase<EffectImpl>
        {
            public enum Props
            {
                TextureSize = 0,
                StepWidth,
                Mode,
                OutlineColor,
                BorderWidth,
                Softness,
                Threshold,
                StyleMode,
                DashLen,
                GapLen,
                ShortDashLen,
                WaveAmp,
                WaveFreq,
                PhaseOffset,
                DoubleGap,
                PositionMode,
            }

            [StructLayout(LayoutKind.Sequential)]
            private struct ConstantBuffer
            {
                public Vector2 TextureSize;
                public float StepWidth;
                public int Mode;
                public Vector4 OutlineColor;
                public float BorderWidth;
                public float Softness;
                public float Threshold;
                public float StyleMode;
                public float DashLen;
                public float GapLen;
                public float ShortDashLen;
                public float WaveAmp;
                public float WaveFreq;
                public float PhaseOffset;
                public float DoubleGap;
                public float PositionMode;
            }

            private ConstantBuffer _cb;

            public EffectImpl() : base(ShaderResourceLoader.GetStrokePS())
            {
                _cb = new ConstantBuffer
                {
                    TextureSize = new Vector2(100, 100),
                    OutlineColor = Vector4.One,
                    Threshold = 0.5f,
                };
            }

            protected override void UpdateConstants()
            {
                drawInformation?.SetPixelShaderConstantBuffer(_cb);
            }

            public override void MapInputRectsToOutputRect(
                RawRect[] inputRects,
                RawRect[] inputOpaqueSubRects,
                out RawRect outputRect,
                out RawRect outputOpaqueSubRect)
            {
                outputRect = inputRects.Length > 0 ? inputRects[0] : default;
                outputOpaqueSubRect = default;
            }

            public override void MapOutputRectToInputRects(
                RawRect outputRect,
                RawRect[] inputRects)
            {
                if (inputRects.Length > 0)
                    inputRects[0] = outputRect;
            }

            [CustomEffectProperty(PropertyType.Vector2, (int)Props.TextureSize)]
            public Vector2 TextureSize { get => _cb.TextureSize; set { _cb.TextureSize = value; UpdateConstants(); } }

            [CustomEffectProperty(PropertyType.Float, (int)Props.StepWidth)]
            public float StepWidth { get => _cb.StepWidth; set { _cb.StepWidth = value; UpdateConstants(); } }

            [CustomEffectProperty(PropertyType.Int32, (int)Props.Mode)]
            public int Mode { get => _cb.Mode; set { _cb.Mode = value; UpdateConstants(); } }

            [CustomEffectProperty(PropertyType.Vector4, (int)Props.OutlineColor)]
            public Vector4 OutlineColor { get => _cb.OutlineColor; set { _cb.OutlineColor = value; UpdateConstants(); } }

            [CustomEffectProperty(PropertyType.Float, (int)Props.BorderWidth)]
            public float BorderWidth { get => _cb.BorderWidth; set { _cb.BorderWidth = value; UpdateConstants(); } }

            [CustomEffectProperty(PropertyType.Float, (int)Props.Softness)]
            public float Softness { get => _cb.Softness; set { _cb.Softness = value; UpdateConstants(); } }

            [CustomEffectProperty(PropertyType.Float, (int)Props.Threshold)]
            public float Threshold { get => _cb.Threshold; set { _cb.Threshold = value; UpdateConstants(); } }

            [CustomEffectProperty(PropertyType.Float, (int)Props.StyleMode)]
            public float StyleMode { get => _cb.StyleMode; set { _cb.StyleMode = value; UpdateConstants(); } }

            [CustomEffectProperty(PropertyType.Float, (int)Props.DashLen)]
            public float DashLen { get => _cb.DashLen; set { _cb.DashLen = value; UpdateConstants(); } }

            [CustomEffectProperty(PropertyType.Float, (int)Props.GapLen)]
            public float GapLen { get => _cb.GapLen; set { _cb.GapLen = value; UpdateConstants(); } }

            [CustomEffectProperty(PropertyType.Float, (int)Props.ShortDashLen)]
            public float ShortDashLen { get => _cb.ShortDashLen; set { _cb.ShortDashLen = value; UpdateConstants(); } }

            [CustomEffectProperty(PropertyType.Float, (int)Props.WaveAmp)]
            public float WaveAmp { get => _cb.WaveAmp; set { _cb.WaveAmp = value; UpdateConstants(); } }

            [CustomEffectProperty(PropertyType.Float, (int)Props.WaveFreq)]
            public float WaveFreq { get => _cb.WaveFreq; set { _cb.WaveFreq = value; UpdateConstants(); } }

            [CustomEffectProperty(PropertyType.Float, (int)Props.PhaseOffset)]
            public float PhaseOffset { get => _cb.PhaseOffset; set { _cb.PhaseOffset = value; UpdateConstants(); } }

            [CustomEffectProperty(PropertyType.Float, (int)Props.DoubleGap)]
            public float DoubleGap { get => _cb.DoubleGap; set { _cb.DoubleGap = value; UpdateConstants(); } }

            [CustomEffectProperty(PropertyType.Float, (int)Props.PositionMode)]
            public float PositionMode { get => _cb.PositionMode; set { _cb.PositionMode = value; UpdateConstants(); } }
        }
    }
}
