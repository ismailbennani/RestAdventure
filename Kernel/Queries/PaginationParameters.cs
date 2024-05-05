namespace RestAdventure.Kernel.Queries;

public class PaginationParameters
{
    public required int PageNumber { get; init; }
    public required int PageSize { get; init; }
}