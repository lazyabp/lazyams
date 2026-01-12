namespace System.Linq;


public static class LazyPagingQueryableExtensions
{
    /// <summary>
    /// Used for paging with an <see cref="IPagedResultRequest"/> object.
    /// </summary>
    /// <param name="query">Queryable to apply paging</param>
    /// <param name="pagedResultRequest">An object implements <see cref="IPagedResultRequest"/> interface</param>
    public static IQueryable<T> PageBy<T>(this IQueryable<T> query, IPagedResultRequest pagedResultRequest)
    {
        pagedResultRequest.PageIndex = int.Max(pagedResultRequest.PageIndex, 1);

        return query.Skip(((pagedResultRequest.PageIndex - 1) * pagedResultRequest.PageSize)).Take(pagedResultRequest.PageSize);
    }
}
