using System;
using System.ComponentModel;

namespace Reactive
{
    public class ReactiveProp<T> : Observable, IDisposable
    {
        public ReactiveProp(Func<T> evaluator, params INotifyPropertyChanged[] dependencies)
        {
            _evaluator = evaluator;
            _dependencies = dependencies;
            Subscribe();
        }

        public T Value
        {
            get => _value;
            private set => SetAndNotify(ref _value, value);
        }
        private T _value;

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
            Value = _evaluator();
        }

        private Func<T> _evaluator;
        private INotifyPropertyChanged[] _dependencies;
    }
}