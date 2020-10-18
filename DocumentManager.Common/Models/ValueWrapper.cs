namespace DocumentManager.Core.Models
{
    public struct ValueWrapper<T>
    {
        public T Value { get; set; }
        public bool IsSuccessful { get; set; }

        public ValueWrapper(T value, bool success)
        {
            Value = value;
            IsSuccessful = success;
        }
    }
}
