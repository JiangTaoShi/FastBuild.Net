

namespace FastBuild.Net.Five.ExpressionBuilder
{
    using FastBuild.Net.Five.Filter;
    using System;
    using System.Collections;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    /// <summary>
    /// 找出所有的判断(Predicative)属性，形成Where 从句，
    /// </summary>
    /// <remarks>
    /// 从属性上取Attribute， 如果一个Attribute都没有指定， 则默认为EqualAttribute;
    /// 如果属性指定了其他Attribute， 比如OrderBy， 且没有指定任何PredicateAttribute， 则不认为是判断属性
    /// </remarks>
    internal class PredicateQueryExpressionBuilder : BaseQueryExpressionBuilder
    {
        public PredicateQueryExpressionBuilder(object queryFilter, ParameterExpression parameter)
            : base(queryFilter, parameter)
        {
            Result = Expression.Constant(true);
        }

        /// <summary>
        /// 生成Where条件判断子句
        /// </summary>
        /// <returns></returns>

        public override Expression Build()
        {
            PropertyInfo[] properties = FilterType.GetProperties(BindingFlagsForRetrieveProperties);
            foreach (PropertyInfo filterProperty in properties)
            {
                object value = filterProperty.GetValue(QueryFilter);

                QueryActionBaseAttribute filterActionAttribute =
                                        filterProperty.GetCustomAttributes<QueryActionBaseAttribute>(false)  // 不继承
                                        .FirstOrDefault()
                                        ;

                if (filterActionAttribute == null)
                {
                    filterActionAttribute = new EqualAttribute(filterProperty.Name);  // 默认使用Equal
                }
                else if (!(filterActionAttribute is PredictiveActionBaseAttribute))
                {
                    // 不是用作Where判断的属性
                    continue;
                }

                if (string.IsNullOrEmpty(filterActionAttribute.ColumnName))
                {
                    filterActionAttribute.ColumnName = filterProperty.Name;  // 默认取属性名称
                }


                if (IsSkipValue(value, (PredictiveActionBaseAttribute)filterActionAttribute))  // 略过设定的跳过值
                {
                    continue;
                }

                InternalBuild(filterProperty, value, filterActionAttribute);
            }

            return Result;
        }

        /// <summary>
        /// 是否为filter的略过值。 如果是， 将不生成Where判断子句
        /// </summary>
        /// <param name="value"></param>
        /// <param name="filterActionAttribute"></param>
        /// <returns></returns>
        private bool IsSkipValue(object value, PredictiveActionBaseAttribute filterActionAttribute)
        {
            if (value == null)
            {
                return filterActionAttribute.SkipWhenValue == null;
            }

            bool result = (value.Equals(filterActionAttribute.SkipWhenValue));
            return result;
        }

        protected void InternalBuild(PropertyInfo filterProperty, object value, QueryActionBaseAttribute filterAction)
        {
            // 例子1:  p=> p.member  == filterValue
            // 例子2 list in:  p=> filterValue.Contains (p.member)
            // 例子3 text like :  p=> p.member.Contains (filterValue)
            //// 左值 
            MemberExpression member;
            try
            {
                member = Expression.PropertyOrField(Parameter, filterAction.ColumnName);
            }
            catch (ArgumentException ex)
            {

                throw new InvalidPropertyFilterException(filterAction.ColumnName, ex);
            }


            // 右值 , 注意做Nullable类型转换

            Expression filterValue;

            try
            {

                filterValue = BuildFilterValue(value, member);
            }
            catch (Exception ex)
            {
                throw new InvalidTypeCastingFilterException(
                    filterAction.ColumnName,
                    value,
                    member.Type,
                    ex);

            }


            // condition1 && condition2 && ......

            Result = Expression.AndAlso(Result, Judge(filterAction, filterProperty, member, filterValue));
        }
        /// <summary>
        /// 形成where子句中的判断表达式
        /// </summary>
        /// <param name="filterAction"></param>
        /// <param name="property"></param>
        /// <param name="member"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        private static Expression Judge(
            QueryActionBaseAttribute filterAction,
            PropertyInfo property,
            Expression member,
            Expression filterValue)
        {
            Func<Expression, Expression, Expression> binaryOperate =
                    CreateBinaryOperation(filterAction);

            if (binaryOperate != null)
            {
                return binaryOperate(member, filterValue);
            }

            if (filterAction is ContainsAttribute)
            {
                return ContainsAction(property, member, filterValue);
            }
            else if (filterAction is NotContainsAttribute)
            {
                return Expression.Not(ContainsAction(property, member, filterValue));

            }

            return Expression.Constant(true);
        }

        /// <summary>
        /// 包含行为。 
        /// 即可以是string的Contains, 将解释成like '%%'
        /// 也可以是集合的Contains, 将解释成in (.....)
        /// </summary>
        /// <param name="property"></param>
        /// <param name="member"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>

        private static Expression ContainsAction(PropertyInfo property, Expression member, Expression filterValue)
        {
            if (property.PropertyType == typeof(string))
            {
                MethodInfo containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                if (containsMethod != null)
                {
                    return Expression.Call(member, containsMethod, filterValue);
                }
            }
            else if (IsCollection(property))  // 是否集合
            {
                Type memberType = GetRealType(member);
                MethodInfo containsMethod = property.PropertyType.GetMethod("Contains", new[] { memberType });
                if (containsMethod != null)
                {
                    return Expression.Call(filterValue, containsMethod, member);
                }
            }

            throw new NotSupportedException($"{property.PropertyType}该类型暂不支持Contains方法");

        }

        /// <summary>
        /// 类型是否集合类型
        /// </summary>
        /// <param name="propertyType"></param>
        /// <returns></returns>
        private static bool IsCollection(PropertyInfo propertyInfo)
        {
            if (propertyInfo.Name == "string")
            {
                return false;
            }

            if (typeof(IEnumerable).IsAssignableFrom(propertyInfo.PropertyType))
            {
                return true;
            }

            return false;
        }


        /// <summary>
        /// 如果是可空类型Nullable<>， 将获取的内部类型
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        private static Type GetRealType(Expression member)
        {
            if (IsNullableType(member.Type))
            {
                return Nullable.GetUnderlyingType(member.Type);
            }

            return member.Type;
        }


        /// <summary>
        /// 是否可控类型Nullable<>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool IsNullableType(Type type)
        {
            return type.IsGenericType
                        && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }


        /// <summary>
        /// 包装常量，使生成的SQL语句中， 将常量值参数化， 
        ///   例如 [p].[GeneratedDate] > @__Item1_0
        ///  1 避免DateTime的毫秒精度跟SQL datetime 不一致的问题
        ///  2 避免潜在的SQL注入问题
        /// </summary>
        /// <param name="node"></param>
        /// <param name="memberType"></param>
        /// <returns></returns>
        protected static Expression WrapConstant(ConstantExpression node, Type memberType)
        {
            Type generictupleType = typeof(Tuple<>);
            Type tupleType = generictupleType.MakeGenericType(memberType);

            //Replace the ConstantExpression to PropertyExpression of Tuple<T>.Item1
            //Entity Framework will parameterize the expression when the expression tree is compiled
            object wrappedObject = Activator.CreateInstance(tupleType, new[] { node.Value });
            Expression result = Expression.Property(Expression.Constant(wrappedObject), "Item1");
            return result;


        }

        /// <summary>
        /// 取Filter的值, 注意, 为了可比, 需要注意Nullable<>的处理 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        private static Expression BuildFilterValue(object value, MemberExpression member)
        {
            Expression result;

            if (value == null)
            {
                result = Expression.Constant(null, member.Type);
            }
            else if (member.Type == typeof(string))
            {
                result = WrapConstant(Expression.Constant(value), member.Type);
            }
            else if (IsNullableType(member.Type)
                 && !(value is IEnumerable)  // 集合类型不需要做类型转换
                 )
            {
                ConstantExpression constant = Expression.Constant(
                    Convert.ChangeType(value, Nullable.GetUnderlyingType(member.Type)));

                //Expression.Convert(
                //   Expression.Constant(Convert.ChangeType(value, Nullable.GetUnderlyingType(member.Type))),
                //   member.Type
                //  );

                result = WrapConstant(constant, member.Type);

            }
            else if (value is IEnumerable)
            {
                result = Expression.Constant(value);
            }
            else
            {
                result = WrapConstant(Expression.Constant(value), member.Type);
            }

            return result;
        }


        /// <summary>
        /// 生产二元操作表达式
        /// </summary>
        /// <param name="filterAction">过滤器操作定义</param>
        /// <returns></returns>
        private static Func<Expression, Expression, Expression> CreateBinaryOperation(
            QueryActionBaseAttribute filterAction)
        {

            if (filterAction == null
                || filterAction is EqualAttribute
                )
            {
                return Expression.Equal;
            }

            if (filterAction is NotEqualAttribute)
            {
                return Expression.NotEqual;
            }

            if (filterAction is GreaterThanAttribute)
            {
                return Expression.GreaterThan;
            }


            if (filterAction is LessThanAttribute)
            {
                return Expression.LessThan;
            }


            if (filterAction is GreaterThanOrEqualAttribute)
            {
                return Expression.GreaterThanOrEqual;
            }


            if (filterAction is LessThanOrEqualAttribute)
            {
                return Expression.LessThanOrEqual;
            }
            return null;
        }
    }
    internal class AnonymousPredicateQueryExpressionBuilder : PredicateQueryExpressionBuilder
    {

        public AnonymousPredicateQueryExpressionBuilder(object queryFilter, ParameterExpression parameter)
            : base(queryFilter, parameter)
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

                /* 通过关键字， 来将其下级条件形成操作， 比如
                    GreatThan:{
                       Age = 10,
                       Length = 99 
                       ......
                    }
                    =》   Age > 10,  Length >99
                    */
                if (IsPredictiveKeyWords(filterProperty.Name))
                {
                    foreach (PropertyInfo subProperty in value.GetType().GetProperties())
                    {

                        QueryActionBaseAttribute subfilterAction = CreateQueryActionAttribute(
                            filterProperty.Name,
                            subProperty.Name);

                        object subValue = subProperty.GetValue(value);
                        InternalBuild(subProperty, subValue, subfilterAction);
                    }

                }
                else if (IsKeyWords(filterProperty.Name))  // 其他关键字，略过不处理
                {
                    continue;
                }
                else
                {
                    QueryActionBaseAttribute filterAction = new EqualAttribute(filterProperty.Name);
                    InternalBuild(filterProperty, value, filterAction);

                }

                return Result;
            }

            return Result;

        }
        private bool IsPredictiveKeyWords(string name)
        {
            bool result = base.PredictiveKeyWords.Contains(name);
            return result;
        }
        private QueryActionBaseAttribute CreateQueryActionAttribute(string actionName, string columnName)
        {
            QueryActionBaseAttribute result;
            switch (actionName)
            {
                case "equal":
                    result = new EqualAttribute(columnName);
                    break;

                case "notequal":
                    result = new NotEqualAttribute(columnName);
                    break;
                case "greaterthan":
                    result = new GreaterThanAttribute(columnName);
                    break;
                case "greaterthanorequal":
                    result = new GreaterThanOrEqualAttribute(columnName);
                    break;
                case "lessthan":
                    result = new LessThanAttribute(columnName);
                    break;
                case "lessthanorequal":
                    result = new LessThanOrEqualAttribute(columnName);
                    break;
                case "contains":
                    result = new ContainsAttribute(columnName);
                    break;
                case "notcontains":
                    result = new NotContainsAttribute(columnName);
                    break;
                default:
                    throw new NotSupportedException();

            }
            return result;

        }



    }
}
