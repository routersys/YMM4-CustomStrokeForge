using CustomStrokeForge.Views;
using System.Windows;
using YukkuriMovieMaker.Commons;

namespace CustomStrokeForge.Attributes
{
    internal sealed class StrokeEditorAttribute : PropertyEditorAttribute2
    {
        public override FrameworkElement Create()
        {
            return new StrokeEditorControl();
        }

        public override void SetBindings(FrameworkElement control, ItemProperty[] itemProperties)
        {
            if (control is StrokeEditorControl editor && itemProperties.Length > 0)
            {
                editor.Effect = itemProperties[0].PropertyOwner as CustomStrokeForgeEffect;
            }
        }

        public override void ClearBindings(FrameworkElement control)
        {
            if (control is StrokeEditorControl editor)
            {
                editor.Effect = null;
            }
        }
    }
}
