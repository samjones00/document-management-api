namespace DocumentManager.Core.Models
{
    public struct ValueWrapper<T>
    {
        public T Value { get; set; }
        public bool IsSuccessful { get; set; }

        public ValueWrapper(bool success)
        {
            Value = default(T);
            IsSuccessful = success;
        }

        public ValueWrapper(T value, bool success)
        {
            Value = value;
            IsSuccessful = success;
        }
    }
}
