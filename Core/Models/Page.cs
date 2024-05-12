namespace Doorfail.Core.Models;

public interface IPaged
{
    #region Properties

    public int PageIndex { get; set; }
    public int TotalPages { get; }
    public int TotalCount { get; set; }
    public bool HasNextPage { get; }
    public bool HasPreviousPage { get; }
    public int PageSize { get; set; }
    public int? Skip { get; set; }
    public int? Top { get; set; }

    #endregion Properties
}

public static class PagedExtensions
{
    public static Page<T> GetPage<T>(this IEnumerable<T> values, int pageIndex, int pageSize, int totalCount, int? skip, int? top)
    {
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        skip ??= 0;
        top ??= pageSize;

        skip *= pageIndex;

        return new Page<T>
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
            TotalCount = totalCount,
            Values = values.Skip(skip.Value).Take(top.Value),
            Skip = skip,
            Top = top
        };
    }

    public static ICollection<Page<T>> AsPages<T>(this IEnumerable<T> allpages, int pageSize)
    {
        // Convert Allpages list to separate pages
        var pages = new List<Page<T>>();
        var pageIndex = 1;
        var skip = 0;
        var top = pageSize;
        var totalCount = allpages.Count();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        var hasNextPage = pageIndex < totalPages;
        while(hasNextPage)
        {
            var page = new Page<T>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = totalCount,
                Values = allpages.Skip(skip).Take(top),
                Skip = skip,
                Top = top
            };
            pages.Add(page);
            pageIndex++;
            skip += top;
            hasNextPage = pageIndex < totalPages;
        }

        return pages;
    }
}

public struct Page<T>(int pageIndex, int pageSize, int totalCount, IEnumerable<T> values) :IPaged
{
    #region Properties

    public int PageIndex { get; set; } = pageIndex;
    public int TotalCount { get; set; } = totalCount;
    public readonly int TotalPages => (int)Math.Ceiling(totalCount / (double)pageSize);
    public readonly bool HasNextPage => PageIndex < TotalPages;
    public readonly bool HasPreviousPage => PageIndex > 1;
    public int PageSize { get; set; } = pageSize;
    public int? Skip { get; set; } = null;
    public int? Top { get; set; } = null;

    public IEnumerable<T> Values { get; set; } = values;

    #endregion Properties

    public override string ToString() => $"Page {PageIndex} of {TotalPages}";
}