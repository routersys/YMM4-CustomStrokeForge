using CustomStrokeForge.ViewModels;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using YukkuriMovieMaker.Commons;

namespace CustomStrokeForge.Views
{
    public partial class StrokeEditorControl : UserControl, IPropertyEditorControl
    {
        private CustomStrokeForgeEffect? _effect;

        private StrokeEditorViewModel ViewModel => (StrokeEditorViewModel)DataContext;

        public event EventHandler? BeginEdit;
        public event EventHandler? EndEdit;

        public new CustomStrokeForgeEffect? Effect
        {
            get => _effect;
            set
            {
                if (ReferenceEquals(_effect, value))
                    return;

                if (_effect is INotifyPropertyChanged oldNpc)
                    oldNpc.PropertyChanged -= OnEffectPropertyChanged;

                _effect = value;

                if (_effect is INotifyPropertyChanged newNpc)
                    newNpc.PropertyChanged += OnEffectPropertyChanged;

                ViewModel.Effect = _effect;
            }
        }

        public StrokeEditorControl()
        {
            InitializeComponent();
            DataContext = new StrokeEditorViewModel();

            ViewModel.BeginEdit += (_, _) => BeginEdit?.Invoke(this, EventArgs.Empty);
            ViewModel.EndEdit += (_, _) => EndEdit?.Invoke(this, EventArgs.Empty);

            Unloaded += OnUnloaded;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (_effect is INotifyPropertyChanged npc)
                npc.PropertyChanged -= OnEffectPropertyChanged;
        }

        private void OnEffectPropertyChanged(object? sender, PropertyChangedEventArgs e) { }

        private void TabStroke_Click(object sender, RoutedEventArgs e) => ViewModel.SelectedTab = 0;
        private void TabPattern_Click(object sender, RoutedEventArgs e) => ViewModel.SelectedTab = 1;
        private void TabAdvanced_Click(object sender, RoutedEventArgs e) => ViewModel.SelectedTab = 2;

        private void PropertiesEditor_BeginEdit(object sender, EventArgs e) =>
            BeginEdit?.Invoke(this, EventArgs.Empty);

        private void PropertiesEditor_EndEdit(object sender, EventArgs e) =>
            EndEdit?.Invoke(this, EventArgs.Empty);

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;
            var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
            {
                RoutedEvent = MouseWheelEvent,
                Source = sender
            };
            RaiseEvent(eventArg);
        }
    }
}
