using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Reactive
{
    public class Prop<T> : INotifyPropertyChanged
    {
        public Prop(T value = default(T))
        {
            Value = value;
        }
        
        public T Value
        {
            get => _value;
            set => SetAndNotify(ref _value, value);
        }
        private T _value;

        public event PropertyChangedEventHandler PropertyChanged;

        private void SetAndNotify(ref T property, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(property, value) == false)
            {
                property = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
