

namespace FastBuild.Five.Filter
{
    using System;
    /// <summary>
    /// EF 查询动作基类
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public abstract class QueryActionBaseAttribute : Attribute
    {
        /// <summary>
        /// 列名，与EF Entity属性名称相对应
        /// </summary>
        public string ColumnName { get; set; }

        protected QueryActionBaseAttribute()
        {

        }

        protected QueryActionBaseAttribute(string columnName)
        {
            ColumnName = columnName;
        }

    }


}
