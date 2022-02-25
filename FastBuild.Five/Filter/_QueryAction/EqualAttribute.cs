

namespace FastBuild.Net.Five.Filter
{

    public class EqualAttribute : PredictiveActionBaseAttribute
    {
        public EqualAttribute()
        {

        }

        public EqualAttribute(string columnName) : base(columnName)
        {
        }

    }
    public class NotEqualAttribute : PredictiveActionBaseAttribute
    {
        public NotEqualAttribute()
        {

        }

        public NotEqualAttribute(string columnName) : base(columnName)
        {
        }

    }

    public class ContainsAttribute : PredictiveActionBaseAttribute
    {
        public ContainsAttribute()
        {

        }

        public ContainsAttribute(string columnName) : base(columnName)
        {
        }
    }

    public class NotContainsAttribute : PredictiveActionBaseAttribute
    {
        public NotContainsAttribute()
        {

        }

        public NotContainsAttribute(string columnName) : base(columnName)
        {
        }
    }


    public class LessThanOrEqualAttribute : PredictiveActionBaseAttribute
    {
        public LessThanOrEqualAttribute()
        {

        }

        public LessThanOrEqualAttribute(string columnName) : base(columnName)
        {
        }
    }

    public class LessThanAttribute : PredictiveActionBaseAttribute
    {
        public LessThanAttribute()
        {

        }

        public LessThanAttribute(string columnName) : base(columnName)
        {
        }
    }

    public class GreaterThanOrEqualAttribute : PredictiveActionBaseAttribute
    {
        public GreaterThanOrEqualAttribute()
        {

        }

        public GreaterThanOrEqualAttribute(string columnName) : base(columnName)
        {
        }
    }

    public class GreaterThanAttribute : PredictiveActionBaseAttribute
    {
        public GreaterThanAttribute()
        {

        }

        public GreaterThanAttribute(string columnName) : base(columnName)
        {
        }
    }



    /// <summary>
    /// 加了这个Attribte的字段， 不生成EF 查询； 保留供其他特殊处理用
    /// </summary>
    public class SpecialOperationAttribute : QueryActionBaseAttribute
    {
        public SpecialOperationAttribute()
        {

        }
    }
}
