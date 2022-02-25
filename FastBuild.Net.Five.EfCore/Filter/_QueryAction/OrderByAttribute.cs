

namespace FastBuild.Net.Five.Filter
{
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <summary>
    /// 排序判断字段属性
    /// </summary>
    /// <remarks>
    ///  如果用在值是NULL的属性上， 则被忽略；
    ///  根据属性值来确定是正序(true)还是倒序(false)， 如果是非boolean类型（且非NULL）， 则使用OrderByAttribute的默认值
    /// </remarks>
    public class OrderByAttribute : QueryActionBaseAttribute
    {
        public OrderByAttribute()
        {

        }

        public OrderByAttribute(string columnName) : base(columnName)
        {
        }

        public bool DefaultAscending { get; set; } = true;
        public int Priority { get; set; } = 5;
    }
}
