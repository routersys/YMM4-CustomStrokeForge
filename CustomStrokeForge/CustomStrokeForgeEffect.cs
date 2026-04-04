using CustomStrokeForge.Attributes;
using CustomStrokeForge.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Windows.Media;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Exo;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin;
using YukkuriMovieMaker.Plugin.Effects;

namespace CustomStrokeForge
{
    [PluginDetails(AuthorName = "routersys")]

    [VideoEffect(nameof(Texts.CustomStrokeForge), [VideoEffectCategories.Composition], [nameof(Texts.TagStroke), nameof(Texts.TagOutline), nameof(Texts.TagCustom)], IsAviUtlSupported = false, ResourceType = typeof(Texts))]
    public sealed class CustomStrokeForgeEffect : VideoEffectBase
    {
        public override string Label => Texts.CustomStrokeForge;

        [JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        [Display(GroupName = nameof(Texts.GroupEditor), ResourceType = typeof(Texts), Order = 0)]
        [StrokeEditor(PropertyEditorSize = PropertyEditorSize.FullWidth)]
        public object? EditorPlaceholder => null;

        public StrokeStyle Style
        {
            get => _style;
            set => Set(ref _style, value);
        }
        private StrokeStyle _style = StrokeStyle.Solid;

        public Animation StrokeWidth { get; } = new Animation(3, 0.5, 50);

        public Color StrokeColor
        {
            get => _strokeColor;
            set => Set(ref _strokeColor, value);
        }
        private Color _strokeColor = Colors.Black;

        public Animation StrokeOpacity { get; } = new Animation(100, 0, 100);

        public StrokePosition Position
        {
            get => _position;
            set => Set(ref _position, value);
        }
        private StrokePosition _position = StrokePosition.Outer;

        public Animation DashLength { get; } = new Animation(10, 1, 200);
        public Animation GapLength { get; } = new Animation(5, 1, 200);
        public Animation ShortDashLength { get; } = new Animation(3, 1, 100);
        public Animation DoubleGap { get; } = new Animation(2, 0.5, 30);
        public Animation WaveAmplitude { get; } = new Animation(3, 0, 50);
        public Animation WaveFrequency { get; } = new Animation(3, 0.1, 100);
        public Animation PhaseOffset { get; } = new Animation(0, -1000, 1000);
        public Animation Softness { get; } = new Animation(0.5, 0, 10);

        private IAnimatable[]? _animatables;

        public override IEnumerable<string> CreateExoVideoFilters(
            int keyFrameIndex,
            ExoOutputDescription exoOutputDescription) => [];

        public override IVideoEffectProcessor CreateVideoEffect(IGraphicsDevicesAndContext devices)
            => new CustomStrokeForgeEffectProcessor(devices, this);

        protected override IEnumerable<IAnimatable> GetAnimatables()
            => _animatables ??= [StrokeWidth, StrokeOpacity, DashLength, GapLength, ShortDashLength, DoubleGap, WaveAmplitude, WaveFrequency, PhaseOffset, Softness];

        public sealed class StrokeParameters(CustomStrokeForgeEffect parent)
        {
            [Display(GroupName = nameof(Texts.GroupStroke), Name = nameof(Texts.StrokeStyleLabel), Description = nameof(Texts.StrokeStyleDesc), ResourceType = typeof(Texts), Order = 100)]
            [EnumComboBox]
            public StrokeStyle Style { get => parent.Style; set => parent.Style = value; }

            [Display(GroupName = nameof(Texts.GroupStroke), Name = nameof(Texts.StrokeWidth), Description = nameof(Texts.StrokeWidthDesc), ResourceType = typeof(Texts), Order = 101)]
            [AnimationSlider("F1", "px", 0.5, 50)]
            public Animation StrokeWidth => parent.StrokeWidth;

            [Display(GroupName = nameof(Texts.GroupStroke), Name = nameof(Texts.StrokeColor), Description = nameof(Texts.StrokeColorDesc), ResourceType = typeof(Texts), Order = 102)]
            [ColorPicker]
            public Color StrokeColor { get => parent.StrokeColor; set => parent.StrokeColor = value; }

            [Display(GroupName = nameof(Texts.GroupStroke), Name = nameof(Texts.StrokeOpacity), Description = nameof(Texts.StrokeOpacityDesc), ResourceType = typeof(Texts), Order = 103)]
            [AnimationSlider("F0", "%", 0, 100)]
            public Animation StrokeOpacity => parent.StrokeOpacity;

            [Display(GroupName = nameof(Texts.GroupStroke), Name = nameof(Texts.StrokePosition), Description = nameof(Texts.StrokePositionDesc), ResourceType = typeof(Texts), Order = 104)]
            [EnumComboBox]
            public StrokePosition Position { get => parent.Position; set => parent.Position = value; }
        }

        public sealed class PatternParameters(CustomStrokeForgeEffect parent)
        {
            [Display(GroupName = nameof(Texts.GroupPattern), Name = nameof(Texts.DashLength), Description = nameof(Texts.DashLengthDesc), ResourceType = typeof(Texts), Order = 200)]
            [AnimationSlider("F1", "px", 1, 100)]
            public Animation DashLength => parent.DashLength;

            [Display(GroupName = nameof(Texts.GroupPattern), Name = nameof(Texts.GapLength), Description = nameof(Texts.GapLengthDesc), ResourceType = typeof(Texts), Order = 201)]
            [AnimationSlider("F1", "px", 1, 100)]
            public Animation GapLength => parent.GapLength;

            [Display(GroupName = nameof(Texts.GroupPattern), Name = nameof(Texts.ShortDashLength), Description = nameof(Texts.ShortDashLengthDesc), ResourceType = typeof(Texts), Order = 202)]
            [AnimationSlider("F1", "px", 1, 50)]
            public Animation ShortDashLength => parent.ShortDashLength;

            [Display(GroupName = nameof(Texts.GroupPattern), Name = nameof(Texts.DoubleGap), Description = nameof(Texts.DoubleGapDesc), ResourceType = typeof(Texts), Order = 203)]
            [AnimationSlider("F1", "px", 0.5, 20)]
            public Animation DoubleGap => parent.DoubleGap;

            [Display(GroupName = nameof(Texts.GroupPattern), Name = nameof(Texts.WaveAmplitude), Description = nameof(Texts.WaveAmplitudeDesc), ResourceType = typeof(Texts), Order = 204)]
            [AnimationSlider("F1", "px", 0, 50)]
            public Animation WaveAmplitude => parent.WaveAmplitude;

            [Display(GroupName = nameof(Texts.GroupPattern), Name = nameof(Texts.WaveFrequency), Description = nameof(Texts.WaveFrequencyDesc), ResourceType = typeof(Texts), Order = 205)]
            [AnimationSlider("F2", "", 0.1, 20)]
            public Animation WaveFrequency => parent.WaveFrequency;

            [Display(GroupName = nameof(Texts.GroupPattern), Name = nameof(Texts.PhaseOffset), Description = nameof(Texts.PhaseOffsetDesc), ResourceType = typeof(Texts), Order = 206)]
            [AnimationSlider("F2", "", -100, 100)]
            public Animation PhaseOffset => parent.PhaseOffset;
        }

        public sealed class AdvancedParameters(CustomStrokeForgeEffect parent)
        {
            [Display(GroupName = nameof(Texts.GroupAdvanced), Name = nameof(Texts.Softness), Description = nameof(Texts.SoftnessDesc), ResourceType = typeof(Texts), Order = 300)]
            [AnimationSlider("F2", "", 0, 10)]
            public Animation Softness => parent.Softness;
        }
    }
}
