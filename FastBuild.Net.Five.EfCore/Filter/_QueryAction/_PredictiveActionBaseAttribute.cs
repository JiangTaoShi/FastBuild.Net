

namespace FastBuild.Net.Five.Filter
{
    /// <summary>
    /// 预测判断， 即Where从句条件
    /// </summary>
    public abstract class PredictiveActionBaseAttribute : QueryActionBaseAttribute
    {
        /// <summary>
        /// 当条件值等于SkipWhenValue时， 该条件将不组成Where子句
        /// </summary>
        public object SkipWhenValue { get; set; } = null;

        protected PredictiveActionBaseAttribute()
        {

        }

        protected PredictiveActionBaseAttribute(string columnName) : base(columnName)
        {
        }
    }
}
