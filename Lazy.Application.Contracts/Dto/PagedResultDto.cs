namespace Lazy.Application.Contracts.Dto;

public class PagedResultDto<T> : ListResultDto<T>
{
    public long Total { get; set; }

    /// <summary>
    /// Creates a new <see cref="PagedResultDto{T}"/> object.
    /// </summary>
    public PagedResultDto()
    {
    }

    /// <summary>
    /// Creates a new <see cref="PagedResultDto{T}"/> object.
    /// </summary>
    /// <param name="totalCount">Total count of Items</param>
    /// <param name="data">List of items in current page</param>
    public PagedResultDto(long total, IReadOnlyList<T> data)
        : base(data)
    {
        Total = total;
    }

    /// <summary>
    /// Creates a new <see cref="PagedResultDto{T}"/> object.
    /// </summary>
    /// <param name="success"></param>
    /// <param name="message"></param>
    /// <param name="total"></param>
    /// <param name="data"></param>
    public PagedResultDto(bool success, string message, long total, IReadOnlyList<T> data)
        : base(success, message, data)
    {
        Total = total;
    }
}
