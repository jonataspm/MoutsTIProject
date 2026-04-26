namespace Ambev.DeveloperEvaluation.Common.Data;

public class PaginatedResult<T>
{
    public IEnumerable<T> Data { get; set; } = new List<T>();
    public int TotalItems { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }

    public PaginatedResult() { }

    public PaginatedResult(IEnumerable<T> data, int totalItems, int pageNumber, int pageSize)
    {
        Data = data;
        TotalItems = totalItems;
        CurrentPage = pageNumber;
        TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
    }
}