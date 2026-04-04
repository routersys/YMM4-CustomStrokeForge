using System.ComponentModel.DataAnnotations;

namespace CustomStrokeForge.Enums
{
    public enum StrokePosition
    {
        [Display(Name = nameof(Texts.PositionOuter), Description = nameof(Texts.PositionOuterDesc), ResourceType = typeof(Texts))]
        Outer,

        [Display(Name = nameof(Texts.PositionCenter), Description = nameof(Texts.PositionCenterDesc), ResourceType = typeof(Texts))]
        Center,

        [Display(Name = nameof(Texts.PositionInner), Description = nameof(Texts.PositionInnerDesc), ResourceType = typeof(Texts))]
        Inner,
    }
}
