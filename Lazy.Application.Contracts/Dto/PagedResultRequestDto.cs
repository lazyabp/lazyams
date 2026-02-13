namespace Lazy.Application.Contracts.Dto;

public class PagedResultRequestDto : IPagedResultRequest, ISortedResultRequest
{
    public int PageIndex { get ; set ; } = 1;
    public int PageSize { get ; set ; } = int.MaxValue;
    public string Sorting { get; set ; }
}
