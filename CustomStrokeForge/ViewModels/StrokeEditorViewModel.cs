using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CustomStrokeForge.ViewModels
{
    internal sealed class StrokeEditorViewModel : INotifyPropertyChanged
    {
        private CustomStrokeForgeEffect? _effect;
        private int _selectedTab;
        private object? _selectedTarget;

        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler? BeginEdit;
        public event EventHandler? EndEdit;

        public CustomStrokeForgeEffect? Effect
        {
            get => _effect;
            set
            {
                if (ReferenceEquals(_effect, value))
                    return;
                _effect = value;
                OnPropertyChanged();
                RefreshTarget();
            }
        }

        public int SelectedTab
        {
            get => _selectedTab;
            set
            {
                if (_selectedTab == value)
                    return;
                _selectedTab = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsStrokeTabActive));
                OnPropertyChanged(nameof(IsPatternTabActive));
                OnPropertyChanged(nameof(IsAdvancedTabActive));
                RefreshTarget();
            }
        }

        public object? SelectedTarget
        {
            get => _selectedTarget;
            private set
            {
                if (ReferenceEquals(_selectedTarget, value))
                    return;
                _selectedTarget = value;
                OnPropertyChanged();
            }
        }

        public bool IsStrokeTabActive => _selectedTab == 0;
        public bool IsPatternTabActive => _selectedTab == 1;
        public bool IsAdvancedTabActive => _selectedTab == 2;

        public void RaiseBeginEdit() => BeginEdit?.Invoke(this, EventArgs.Empty);
        public void RaiseEndEdit() => EndEdit?.Invoke(this, EventArgs.Empty);

        private void RefreshTarget()
        {
            if (_effect is null)
            {
                SelectedTarget = null;
                return;
            }

            SelectedTarget = _selectedTab switch
            {
                0 => new CustomStrokeForgeEffect.StrokeParameters(_effect),
                1 => new CustomStrokeForgeEffect.PatternParameters(_effect),
                2 => new CustomStrokeForgeEffect.AdvancedParameters(_effect),
                _ => null,
            };
        }

        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
