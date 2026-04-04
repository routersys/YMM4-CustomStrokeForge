using System.ComponentModel.DataAnnotations;

namespace CustomStrokeForge.Enums
{
    public enum StrokeStyle
    {
        [Display(Name = nameof(Texts.StyleSolid), Description = nameof(Texts.StyleSolidDesc), ResourceType = typeof(Texts))]
        Solid,

        [Display(Name = nameof(Texts.StyleDotted), Description = nameof(Texts.StyleDottedDesc), ResourceType = typeof(Texts))]
        Dotted,

        [Display(Name = nameof(Texts.StyleDashed), Description = nameof(Texts.StyleDashedDesc), ResourceType = typeof(Texts))]
        Dashed,

        [Display(Name = nameof(Texts.StyleChain), Description = nameof(Texts.StyleChainDesc), ResourceType = typeof(Texts))]
        Chain,

        [Display(Name = nameof(Texts.StyleDouble), Description = nameof(Texts.StyleDoubleDesc), ResourceType = typeof(Texts))]
        Double,

        [Display(Name = nameof(Texts.StyleWave), Description = nameof(Texts.StyleWaveDesc), ResourceType = typeof(Texts))]
        Wave,
    }
}
