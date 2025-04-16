namespace FastPackForShare.Models;

public class QueryResultModel<TEntity> where TEntity : class
{
    public int Count { get; set; }
    public IEnumerable<TEntity> Result { get; set; }
}
