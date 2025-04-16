using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

    public static string AddFilters(List<PagedFilterModel> sqlConditions)
    {
        string filters = string.Empty;

        sqlConditions.ForEach(condition =>
        {
            if (condition.Value != null)
            {
                filters += $@"
                        AND {condition.Column} ";

                if (condition.Value.GetType() == typeof(string))
                    filters += $"LIKE '%' + {condition.Parameter} + '%'";
                else if (condition.Value is IEnumerable<int> || condition.Value is IEnumerable<string>)
                    filters += $"IN {condition.Parameter}";
                else
                    filters += $"= {condition.Parameter}";
            }
        });

        return filters;
    }

    public static string GetPagedTSqlPagination(string query, int page, int pageSize)
    {
        var newQuery = $"{query} " +
                       $"OFFSET ({page}-1) * {pageSize} ROWS " +
                       $"FETCH NEXT {pageSize} ROWS ONLY";

        return newQuery;
    }

    public static string AddFieldsToUpdateQuery(IEnumerable<PagedFilterModel> parameters)
    {
        List<string> arrParameters = new List<string>();

        foreach (var param in parameters)
        {
            Type paramType = param.GetType();
            bool objectString = paramType == typeof(string) && !string.IsNullOrEmpty((string)param.Value);
            bool objectInt = paramType == typeof(int) && (int)param.Value > 0;
            bool objectBool = paramType == typeof(bool);

            if (objectString)
                arrParameters.Add($@"{param.Parameter} = @{param.Parameter}");
            else if (objectInt)
                arrParameters.Add($@"{param.Parameter} = @{param.Parameter}");
            else if (objectBool)
                arrParameters.Add($@"{param.Parameter} = @{param.Parameter}");
        }

        return arrParameters.Count > 0 ? string.Join(',', arrParameters) : string.Empty;
    }

    public static int GetDefaultPageIndex(int? pageIndex) => pageIndex.HasValue ? pageIndex.Value : 1;
    public static int GetDefaultPageSize(int? pageSize) => pageSize.HasValue ? pageSize.Value : 10;
    private static int GetPagination(int page, int pageSize) => (page - 1) * pageSize;
}
