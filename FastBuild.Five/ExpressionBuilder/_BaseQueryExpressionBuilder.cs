


namespace FastBuild.Five.ExpressionBuilder
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    /// 基类，根据查询条件（QueryFilter）构建查询表达式
    /// </summary>
    internal abstract class BaseQueryExpressionBuilder
    {
        /// <summary>查询条件</summary>
        public object QueryFilter { get; }

        /// <summary>查询条件的对象类型</summary>
        protected Type FilterType { get; }


        /// <summary>lamda表达式的参数（单） 例如  p=>p.Name=="Harvey" , 表示 p</summary>
        public ParameterExpression Parameter { get; }


        /// <summary>表达式结果， 例如  p=>p.Name=="Harvey" </summary>
        public Expression Result { get; protected set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="queryFilter">查询条件</param>
        /// <param name="parameter">lamda表达式的参数</param>
        protected BaseQueryExpressionBuilder(object queryFilter, ParameterExpression parameter)
        {
            QueryFilter = queryFilter;
            FilterType = queryFilter.GetType();
            Parameter = parameter;
        }


        /// <summary>构建表达式， 并返回Result对象</summary>
        public abstract Expression Build();


        protected readonly BindingFlags BindingFlagsForRetrieveProperties =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty;

        protected readonly IList<string> PredictiveKeyWords = new List<string> { "Equal", "NotEqual", "GreatThan", "GreatThanOrEqual", "LessThan", "LessThanOrEqual", "Contains", "NotContains" };

        protected readonly IList<string> OrderByKeyWords = new List<string> { "OrderBy" };
        protected readonly IList<string> PagerKeyWords = new List<string> { "Pager" };

        protected bool IsKeyWords(string word)
        {
            if (PredictiveKeyWords.Contains(word))
            {
                return true;
            }

            if (OrderByKeyWords.Contains(word))
            {
                return true;
            }

            if (PagerKeyWords.Contains(word))
            {
                return true;
            }

            return false;
        }
    }

}
