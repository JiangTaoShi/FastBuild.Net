

namespace FastBuild.Five.ExpressionBuilder
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;


    internal class OrderByQueryableVisitor<T> : BaseQueryableVisitor<T>
    {
        public OrderByQueryableVisitor(IQueryable<T> queryable) : base(queryable)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <remarks>支持多个OrderBy</remarks>
        public override IQueryable<T> Visit(object queryFilter)
        {

            ParameterExpression parameter = Expression.Parameter(typeof(T), "p");//创建参数p

            Type filterType = queryFilter.GetType();
            KeySelectorExpressionBuilder builder;
            if (CheckIfAnonymousType(filterType))
            {
                builder = new AnonymousKeySelectorExpressionBuilder(queryFilter, parameter);
            }
            else
            {

                builder = new KeySelectorExpressionBuilder(queryFilter, parameter);
            }

            builder.Build();
            IQueryable<T> result = queryable;
            foreach (OrderByDescription item in builder.Results.OrderByDescending(p => p.Priority))
            {
                Expression<Func<T, object>> lambda = Expression.Lambda<Func<T, object>>(item.Body, parameter);
                if (item.Asc)
                {
                    result = result.OrderBy(lambda);
                }
                else
                {
                    result = result.OrderByDescending(lambda);
                }
            }
            return result;
        }
    }


}
