namespace Reactive
{
    public class Prop<T> : Observable
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
    }
}
