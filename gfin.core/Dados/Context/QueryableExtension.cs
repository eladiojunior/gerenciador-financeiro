using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GFin.Dados
{
    public static class QueryableExtension
    {
        private static IOrderedQueryable<TSource> SortOrderBy<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, object>> keySelector, string sortOrderBy)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (keySelector == null)
                throw new ArgumentNullException("keySelector");
            Expression body = keySelector.Body;
            if (body.NodeType == ExpressionType.Convert)
                body = ((UnaryExpression)body).Operand;

            LambdaExpression keySelectorLambda = Expression.Lambda(body, keySelector.Parameters);
            Type tkey = keySelectorLambda.ReturnType;
            MethodInfo orderbyMethod = (from x in typeof(Queryable).GetMethods()
                                        where x.Name == sortOrderBy
                                        let parameters = x.GetParameters()
                                        where parameters.Length == 2
                                        let generics = x.GetGenericArguments()
                                        where generics.Length == 2
                                        where parameters[0].ParameterType == typeof(IQueryable<>).MakeGenericType(generics[0]) &&
                                            parameters[1].ParameterType == typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(generics[0], generics[1]))
                                        select x).Single();
            return (IOrderedQueryable<TSource>)source.Provider.CreateQuery<TSource>(
                Expression.Call(null, orderbyMethod.MakeGenericMethod(new Type[] { 
                    typeof(TSource), tkey 
                }), new Expression[] { 
                    source.Expression, Expression.Quote(keySelectorLambda) 
                }));
        }

        public static IOrderedQueryable<TSource> OrderByAsc<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, object>> keySelector)
        {
            return SortOrderBy(source, keySelector, "OrderBy");
        }

        public static IOrderedQueryable<TSource> OrderByDesc<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, object>> keySelector)
        {
            return SortOrderBy(source, keySelector, "OrderByDescending");
        }
    }

}
