namespace Lazy.Core.WrapperResult;

public class ApiResponseResult
{
    public bool Success { get; set; }
    public int Status { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime Time { get; set; }
}


public class ApiResponseResult<T> : ApiResponseResult
{
    public T Data { get; set; }
}