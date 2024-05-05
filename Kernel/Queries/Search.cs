namespace RestAdventure.Kernel.Queries;

public static class Search
{
    public static SearchResult<TResult> Paginate<TResult>(IEnumerable<TResult> source, PaginationParameters pagination)
    {
        int totalCount = source.Count();

        if (pagination.PageSize == 0)
        {
            return new SearchResult<TResult>
            {
                Items = Array.Empty<TResult>(),
                PageNumber = 0,
                PageSize = 0,
                TotalItemsCount = totalCount,
                TotalPagesCount = 0
            };
        }

        int totalPagesCount = (int)Math.Ceiling((float)totalCount / pagination.PageSize);
        int toSkip = pagination.PageSize * (pagination.PageNumber - 1);

        return new SearchResult<TResult>
        {
            Items = source.Skip(toSkip).Take(pagination.PageSize).ToArray(),
            PageNumber = pagination.PageNumber,
            PageSize = pagination.PageSize,
            TotalItemsCount = totalCount,
            TotalPagesCount = totalPagesCount
        };
    }
}
