
namespace FastBuild.Net.Five.ExpressionBuilder
{
    using FastBuild.Net.Five.Filter;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    /// <summary>
    /// 构建OrderBy从句表达式， 可返回多个字段排序， 以Results为准
    /// </summary>
    internal class KeySelectorExpressionBuilder : BaseQueryExpressionBuilder
    {
        public KeySelectorExpressionBuilder(object queryFilter, ParameterExpression parameter)
            : base(queryFilter, parameter)
        {

        }

        public override Expression Build()
        {
            PropertyInfo[] properties = FilterType.GetProperties(BindingFlagsForRetrieveProperties);
            foreach (PropertyInfo filterProperty in properties)
            {

                OrderByAttribute filterOperation =
                                         filterProperty.GetCustomAttribute<OrderByAttribute>(false)
                                        ;
                if (filterOperation == null)
                {
                    continue;
                }

                if (string.IsNullOrEmpty(filterOperation.ColumnName))
                {
                    filterOperation.ColumnName = filterProperty.Name;  // 默认取属性名称
                }

                object value = filterProperty.GetValue(QueryFilter);
                if (value == null)
                {
                    continue;
                }
                InternalBuild(filterProperty, value, filterOperation);

            }
            return Result;
        }

        protected void InternalBuild(PropertyInfo filterProperty, object value, OrderByAttribute filterOperation)
        {
            // 例子 :  p=> p.member  
            // 属性值 
            Expression member = Expression.Convert(Expression.PropertyOrField(Parameter, filterOperation.ColumnName)
              , typeof(object));

            Result = member;   // 最后一个排序

            Asc = GetAscendingValue(value, filterOperation.DefaultAscending);
            Priority = filterOperation.Priority;

            Results.Add(new OrderByDescription(Result, Asc, Priority));
        }

        protected bool GetAscendingValue(object value, bool defaultAscending)
        {
            if (value == null)
            {
                return defaultAscending;
            }


            if (bool.TryParse(value.ToString(), out bool result))
            {
                return result;
            }

            return defaultAscending;
        }

        public bool Asc { get; private set; }

        public int Priority { get; private set; }

        public IList<OrderByDescription> Results { get; private set; } = new List<OrderByDescription>();

    }

    internal class AnonymousKeySelectorExpressionBuilder : KeySelectorExpressionBuilder
    {
        public AnonymousKeySelectorExpressionBuilder(object queryFilter, ParameterExpression parameter) : base(queryFilter, parameter)
        {
        }

        public override Expression Build()
        {
            PropertyInfo[] properties = FilterType.GetProperties();


            foreach (PropertyInfo filterProperty in properties)
            {
                object value = filterProperty.GetValue(QueryFilter);
                if (value == null)
                {
                    continue;
                }

                if (IsOrderByKeyWords(filterProperty.Name))
                {
                    foreach (PropertyInfo subProperty in value.GetType().GetProperties())
                    {

                        OrderByAttribute subFilterOperation = CreateOperationAttribute(
                            filterProperty.Name,
                            subProperty.Name);

                        object subValue = subProperty.GetValue(value);

                        InternalBuild(subProperty, subValue, subFilterOperation);
                    }
                    return Result;

                }
            }

            return Result;

        }



        private OrderByAttribute CreateOperationAttribute(string action, string columnName)
        {
            return new OrderByAttribute(columnName);
        }

        private bool IsOrderByKeyWords(string name)
        {
            return OrderByKeyWords.Contains(name);
        }
    }

    internal class OrderByDescription
    {


        public OrderByDescription(Expression result, bool asc, int priority)
        {
            Body = result;
            Asc = asc;
            Priority = priority;
        }

        public Expression Body { get; set; }
        public bool Asc { get; set; }
        public int Priority { get; set; }
    }

}
