

namespace FastBuild.Five.ExpressionBuilder
{
    using FastBuild.Five.Filter;
    using System.Linq;
    using System.Reflection;

    internal class SkipQueryableVisitor<T> : BaseQueryableVisitor<T>
    {
        public SkipQueryableVisitor(IQueryable<T> queryable) : base(queryable)
        {

        }

        public override IQueryable<T> Visit(object queryFilter)
        {

            PropertyInfo[] properties = queryFilter.GetType().GetProperties(BindingFlagsForRetrieveProperties);

            foreach (PropertyInfo property in properties)
            {
                PagerSkipAttribute pageTake = property.GetCustomAttribute<PagerSkipAttribute>();
                if (pageTake != null)
                {
                    int count = GetSkipCount(property.GetValue(queryFilter), pageTake.DefaultSkipCount);
                    IQueryable<T> result = queryable.Skip(count);
                    return result;
                }
            }

            return queryable;
        }

        private int GetSkipCount(object propertyValue, int defaultCount)
        {
            if (propertyValue == null)
            {
                return defaultCount;
            }

            if (int.TryParse(propertyValue.ToString(), out int result))
            {
                if (result > 0)
                {
                    return result;

                }
            }
            return defaultCount;
        }
    }

}
