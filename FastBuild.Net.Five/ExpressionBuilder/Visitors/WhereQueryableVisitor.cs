
namespace FastBuild.Net.Five.ExpressionBuilder
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    internal class WhereQueryableVisitor<T> : BaseQueryableVisitor<T>
    {
        public WhereQueryableVisitor(IQueryable<T> queryable) : base(queryable)
        {

        }

        /// <summary>
        /// Visitor模式， 通过queryFilter构建EF查询表达式
        /// </summary>
        /// <param name="queryFilter">查询条件</param>
        public override IQueryable<T> Visit(object queryFilter)
        {
            Type filterType = queryFilter.GetType();

            ParameterExpression parameter = Expression.Parameter(typeof(T), "p");//创建参数p

            Expression body;

            if (CheckIfAnonymousType(filterType))
            {
                body = new AnonymousPredicateQueryExpressionBuilder(queryFilter, parameter).Build();
            }
            else
            {
                body = new PredicateQueryExpressionBuilder(queryFilter, parameter).Build();
            }
            IQueryable<T> result = queryable.Where(Expression.Lambda<Func<T, bool>>(body, parameter));
            return result;
        }
    }

}
