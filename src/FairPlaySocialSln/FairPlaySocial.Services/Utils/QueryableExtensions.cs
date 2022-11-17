using FairPlaySocial.Models.Filtering;
using FairPlaySocial.Models.Sorting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.Services.Utils
{
    /// <summary>
    /// Based on answer here: https://stackoverflow.com/questions/46306955/how-to-construct-a-dynamic-where-filter-in-ef-core-to-handle-equals-like-gt-l
    /// </summary>
    public static partial class QueryableExtensions
    {
        public static IQueryable<T> Where<T>(this IQueryable<T> source, string propertyName, ComparisonOperator comparison, string value)
        {
            return source.Where(ExpressionUtils.BuildPredicate<T>(propertyName, comparison, value));
        }

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, IEnumerable<SortingModel> sortModels)
        {
            var expression = source.Expression;
            int count = 0;
            foreach (var item in sortModels)
            {
                var parameter = Expression.Parameter(typeof(T), "x");
                var selector = Expression.PropertyOrField(parameter, item!.ColumnName!);
                var method = string.Equals(item.Sort, "desc", StringComparison.OrdinalIgnoreCase) ?
                    (count == 0 ? "OrderByDescending" : "ThenByDescending") :
                    (count == 0 ? "OrderBy" : "ThenBy");
                expression = Expression.Call(typeof(Queryable), method,
                    new Type[] { source.ElementType, selector.Type },
                    expression, Expression.Quote(Expression.Lambda(selector, parameter)));
                count++;
            }
            return count > 0 ? source.Provider.CreateQuery<T>(expression) : source;
        }
    }
}
