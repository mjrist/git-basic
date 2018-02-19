using System;
using System.ComponentModel;

namespace Reactive
{
    public class ReactiveAction : IDisposable
    {
        public ReactiveAction(Action action, params INotifyPropertyChanged[] dependencies)
        {
            _action = action;
            _dependencies = dependencies;
            Subscribe();
        }        

        public void Dispose()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            foreach (var dependency in _dependencies)
            {
                dependency.PropertyChanged += Dependency_PropertyChanged;
            }
        }        

        private void Unsubscribe()
        {
            foreach (var dependency in _dependencies)
            {
                dependency.PropertyChanged -= Dependency_PropertyChanged;
            }
        }

        private void Dependency_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _action?.Invoke();
        }

        private Action _action;
        private INotifyPropertyChanged[] _dependencies;
    }
}
