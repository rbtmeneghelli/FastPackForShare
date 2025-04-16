using FastPackForShare.Default;

namespace FastPackForShare.Services.Bases;

public static class BasePagedResultService
{
    public static BasePagedResultModel<T> GetPaged<T>(this IQueryable<T> query, int? paramPage, int? paramSize) where T : class
    {
        int page = paramPage.HasValue ? paramPage.Value : 1;
        int pageSize = paramSize.HasValue ? paramSize.Value : 10;

        var result = new BasePagedResultModel<T>();
        result.Page = ++page;
        result.PageSize = pageSize;
        result.Results = query?.Count() > 0 ? query.Skip(GetPagination(result.Page, result.PageSize))
                                                   .Take(pageSize)
                                                   .AsEnumerable()
                                                   : Enumerable.Empty<T>();
        result.TotalRecords = result.Results.Count();
        result.NextPage = result.PageSize * result.Page >= result.TotalRecords ? null : (int?)result.Page + 1;
        result.PageCount = result.TotalRecords > 0 ? (int)Math.Ceiling((double)result.TotalRecords / result.PageSize) : 1;
        return result;
    }

    public static string GetPagedTSqlPagination(string query, int page, int pageSize)
    {
        var newQuery = $"{query} " +
                       $"OFFSET ({page}-1) * {pageSize} ROWS " +
                       $"FETCH NEXT {pageSize} ROWS ONLY";

        return newQuery;
    }

    public static int GetDefaultPageIndex(int? pageIndex) => pageIndex.HasValue ? pageIndex.Value : 1;
    public static int GetDefaultPageSize(int? pageSize) => pageSize.HasValue ? pageSize.Value : 10;
    private static int GetPagination(int page, int pageSize) => (page - 1) * pageSize;
}
