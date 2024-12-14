namespace Calmska.Models.Models
{
    public class OperationResult
    {
        public bool Result { get; set; }
        public string Error { get; set; } = string.Empty;
    }

    public class OperationResultT<T>
    {
        public T? Result { get; set; }
        public string Error { get; set; } = string.Empty;
    }
}
