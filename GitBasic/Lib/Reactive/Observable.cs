using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Reactive
{
    public abstract class Observable : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Calls property changed.
        /// </summary>
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Sets the property and calls property changed if the property does not equal the new value.
        /// </summary>
        /// <typeparam name="T"></typeparam>        
        protected void SetAndNotify<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(property, value) == false)
            {
                property = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
