using System.Data.Common;
using System.Data;
using System.Dynamic;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace FastPackForShare.Extensions;

public static class ContextExtension
{
    public static IEnumerable<dynamic> CollectionFromSql(this DbContext dbContext, string Sql, Dictionary<string, object> Parameters)
    {
        using (var cmd = dbContext.Database.GetDbConnection().CreateCommand())
        {
            cmd.CommandText = Sql;
            if (cmd.Connection.State != ConnectionState.Open)
                cmd.Connection.Open();

            foreach (KeyValuePair<string, object> param in Parameters)
            {
                DbParameter dbParameter = cmd.CreateParameter();
                dbParameter.ParameterName = param.Key;
                dbParameter.Value = param.Value;
                cmd.Parameters.Add(dbParameter);
            }

            using (var dataReader = cmd.ExecuteReader())
            {
                while (dataReader.Read())
                {
                    var dataRow = GetDataRow(dataReader);
                    yield return dataRow;
                }
            }
        }
    }

    public static void ApplyGlobalFilters<TInterface>(this ModelBuilder modelBuilder, Expression<Func<TInterface, bool>> expression)
    {
        var entities = modelBuilder.Model
            .GetEntityTypes()
            .Select(e => e.ClrType);

        foreach (Type entity in entities)
        {
            if (entity.IsSubclassOf(typeof(TInterface)))
            {
                var newParam = Expression.Parameter(entity);
                var newbody = ReplacingExpressionVisitor.Replace(expression.Parameters.Single(), newParam, expression.Body);
                modelBuilder.Entity(entity).HasQueryFilter(Expression.Lambda(newbody, newParam));
            }
        }
    }

    private static dynamic GetDataRow(DbDataReader dataReader)
    {
        var dataRow = new ExpandoObject() as IDictionary<string, object>;
        for (var fieldCount = 0; fieldCount < dataReader.FieldCount; fieldCount++)
            dataRow.Add(dataReader.GetName(fieldCount), dataReader[fieldCount]);
        return dataRow;
    }
}
