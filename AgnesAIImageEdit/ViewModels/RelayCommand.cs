using System;
using System.Windows.Input;

namespace AgnesAIImageEdit.ViewModels
{
    public class RelayCommand : ICommand
    {
        private readonly Func<object?, Task>? _async;
        private readonly Action<object?>? _sync;
        private bool _executing;

        public RelayCommand(Action<object?> execute) => _sync = execute;
        public RelayCommand(Func<object?, Task> executeAsync) => _async = executeAsync;

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => !_executing;

        public async void Execute(object? parameter)
        {
            if (_executing) return;
            _executing = true;
            RaiseCanExecuteChanged();
            try
            {
                if (_async != null) await _async(parameter);
                else _sync?.Invoke(parameter);
            }
            finally
            {
                _executing = false;
                RaiseCanExecuteChanged();
            }
        }

        private void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
