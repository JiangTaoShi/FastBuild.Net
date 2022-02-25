

namespace FastBuild.Five.Filter
{
    /// <summary>
    /// 分页返回Take
    /// </summary>
    public class PagerTakeAttribute : QueryActionBaseAttribute
    {
        public PagerTakeAttribute(int defaultCount)
        {
            DefaultTakeCount = defaultCount;
        }

        /// <summary>
        /// 默认返回的行数
        /// </summary>
        public int DefaultTakeCount { get; }
    }

}
