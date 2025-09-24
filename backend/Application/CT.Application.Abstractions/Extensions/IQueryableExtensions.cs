using CT.Application.Abstractions.Enums;
using CT.Application.Abstractions.QueryParameters;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace CT.Application.Abstractions.Extensions;

public static class IQueryableExtensions
{
    public static IQueryable<T> OrderBySortParameters<T>(this IQueryable<T> query, SortQueryParameters? sortParameters)
    {
        if (sortParameters == null)
        {
            return query;
        }

        for (int i = 0; i < sortParameters.Count; i++)
        {
            SortQueryParameter sortParameter = sortParameters[i];
            string text = sortParameter.FieldName.FirstCharToUpperInvariant();
            
            //_ = typeof(T).FullName + "." + text;
            
            Type? propertyType = GetPropertyTypeCached<T>(text);

            if (propertyType != null)
            {
                string key = $"{typeof(T).FullName}.{"GetPropertySelectorExpression"}.{propertyType.FullName}";
                object obj = GenericMethodCache.GetOrAdd(key, ctx => typeof(IQueryableExtensions).GetMethod("GetPropertySelectorExpression")!.MakeGenericMethod(typeof(T), propertyType)).Invoke(null, [text])!;
                string key2 = $"{typeof(T).FullName}.{"OrderByDirection"}.{propertyType.FullName}";
                query = (IQueryable<T>)GenericMethodCache.GetOrAdd(key2, ctx => typeof(IQueryableExtensions).GetMethod("OrderByDirection")!.MakeGenericMethod(typeof(T), propertyType)).Invoke(null, [query, obj, sortParameter.Direction])!;
            }
        }

        return query;
    }

    private static readonly ConcurrentDictionary<string, Type?> PropertyCache = new();
    private static readonly ConcurrentDictionary<string, MethodInfo> GenericMethodCache = new();
    private static readonly ConcurrentDictionary<string, object> PropertySelectorExpressionCache = new();

    public static Expression<Func<T, TProperty>> GetPropertySelectorExpression<T, TProperty>(this string propertyName)
    {
        string propertyName2 = propertyName;
        string key = typeof(T).FullName + "." + propertyName2;
        return (Expression<Func<T, TProperty>>)PropertySelectorExpressionCache.GetOrAdd(key, delegate
        {
            ParameterExpression parameterExpression = Expression.Parameter(typeof(T));
            return Expression.Lambda<Func<T, TProperty>>(Expression.Property(parameterExpression, typeof(T), propertyName2), new ParameterExpression[1] { parameterExpression });
        });
    }
    private static Type? GetPropertyTypeCached<T>(string propertyName)
    {
        string propertyName2 = propertyName;
        string key = typeof(T).FullName + "." + propertyName2;
        return PropertyCache.GetOrAdd(key, x => typeof(T).GetProperty(propertyName2)?.PropertyType);
    }

    public static IQueryable<T> OrderByDirection<T, TProperty>(this IQueryable<T> query, Expression<Func<T, TProperty>> propSelector, SortDirection sortDirection)
    {
        if (query.IsOrdered())
        {
            IOrderedQueryable<T> source = (query as IOrderedQueryable<T>)!;
            query = ((sortDirection == SortDirection.Asc) ? source.ThenBy(propSelector) : source.ThenByDescending(propSelector));
        }
        else
        {
            query = ((sortDirection == SortDirection.Asc) ? query.OrderBy(propSelector) : query.OrderByDescending(propSelector));
        }

        return query;
    }

    public static bool IsOrdered<T>(this IQueryable<T> query)
    {
        ArgumentNullException.ThrowIfNull(query);

        return query.Expression.Type == typeof(IOrderedQueryable<T>);
    }
}
