namespace Lazy.Application.Contracts.Dto;

public class ListResultDto<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;

    public IReadOnlyList<T> Data
    {
        get { return _data ?? (_data = new List<T>()); }
        set { _data = value; }
    }

    private IReadOnlyList<T> _data;

    /// <summary>
    /// Creates a new <see cref="ListResultDto{T}"/> object.
    /// </summary>
    public ListResultDto()
    {
    }

    /// <summary>
    /// Creates a new <see cref="ListResultDto{T}"/> object.
    /// </summary>
    /// <param name="data">List of data</param>
    public ListResultDto(IReadOnlyList<T> data)
    {
        Success = true;
        Message = "Successfully";
        Data = data;
    }

    /// <summary>
    /// Creates a new <see cref="ListResultDto{T}"/> object.
    /// </summary>
    /// <param name="success"></param>
    /// <param name="message"></param>
    /// <param name="data"></param>
    public ListResultDto(bool success, string message, IReadOnlyList<T> data)
    {
        Success = success;
        Message = message;
        Data = data;
    }
}
