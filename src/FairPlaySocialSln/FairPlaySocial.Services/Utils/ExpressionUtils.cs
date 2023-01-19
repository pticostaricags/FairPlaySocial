using FairPlaySocial.Common.CustomExceptions;
using FairPlaySocial.Models.Filtering;
using System.Linq.Expressions;

namespace FairPlaySocial.Services.Utils
{
    /// <summary>
    /// Based on answer here: https://stackoverflow.com/questions/46306955/how-to-construct-a-dynamic-where-filter-in-ef-core-to-handle-equals-like-gt-l
    /// </summary>
    public static partial class ExpressionUtils
    {
        private static string ConvertComparisonOperatorToString(ComparisonOperator comparison) => comparison switch
        {
            ComparisonOperator.Equals => "==",
            ComparisonOperator.NotEquals => "!=",
            ComparisonOperator.GreaterThan => ">",
            ComparisonOperator.LessThan => "<",
            ComparisonOperator.LessThanOrEqual => "<=",
            ComparisonOperator.GreaterThanOrEqual => ">=",
            ComparisonOperator.Contains => "Contains",
            ComparisonOperator.StartsWith => "StartsWith",
            ComparisonOperator.EndsWith => "EndsWith",
            _ => throw new CustomValidationException($"Invalid comparison operator: {comparison}"),
        };
        public static Expression<Func<T, bool>> BuildPredicate<T>(string propertyName,
            ComparisonOperator comparison, string value)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var left = propertyName.Split('.').Aggregate((Expression)parameter, Expression.Property);
            var body = MakeComparison(left, ConvertComparisonOperatorToString(comparison), value);
            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        private static Expression MakeComparison(Expression left, string comparison, string value) => comparison switch
        {
            "==" => MakeBinary(ExpressionType.Equal, left, value),
            "!=" => MakeBinary(ExpressionType.NotEqual, left, value),
            ">" => MakeBinary(ExpressionType.GreaterThan, left, value),
            ">=" => MakeBinary(ExpressionType.GreaterThanOrEqual, left, value),
            "<" => MakeBinary(ExpressionType.LessThan, left, value),
            "<=" => MakeBinary(ExpressionType.LessThanOrEqual, left, value),
            "Contains" or "StartsWith" or "EndsWith" => Expression.Call(MakeString(left), comparison, Type.EmptyTypes, Expression.Constant(value, typeof(string))),
            _ => throw new NotSupportedException($"Invalid comparison operator '{comparison}'."),
        };

        private static Expression MakeString(Expression source)
        {
            return source.Type == typeof(string) ? source : Expression.Call(source, "ToString", Type.EmptyTypes);
        }

        private static Expression MakeBinary(ExpressionType type, Expression left, string value)
        {
            object? typedValue = value;
            if (left.Type != typeof(string))
            {
                if (string.IsNullOrEmpty(value))
                {
                    typedValue = null;
                    if (Nullable.GetUnderlyingType(left.Type) == null)
                        left = Expression.Convert(left, typeof(Nullable<>).MakeGenericType(left.Type));
                }
                else
                {
                    var valueType = Nullable.GetUnderlyingType(left.Type) ?? left.Type;
                    typedValue = valueType.IsEnum ? Enum.Parse(valueType, value) :
                        valueType == typeof(Guid) ? Guid.Parse(value) :
                        Convert.ChangeType(value, valueType);
                }
            }
            var right = Expression.Constant(typedValue, left.Type);
            return Expression.MakeBinary(type, left, right);
        }

    }
}