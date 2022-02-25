

namespace FastBuild.Five.Filter
{
    /// <summary>
    /// 分页计算跳过行数
    /// </summary>
    public class PagerSkipAttribute : QueryActionBaseAttribute
    {
        public PagerSkipAttribute()
        {

        }

        /// <summary>
        /// 默认跳过的行数
        /// </summary>
        public int DefaultSkipCount { get; }
    }

}
