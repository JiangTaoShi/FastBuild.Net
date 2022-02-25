


namespace FastBuild.Five.ExpressionBuilder
{
    using FastBuild.Five.DataObject;
    using FastBuild.Five.Filter;
    using System.Linq;
    using System.Reflection;

    internal class TakeQueryableVisitor<T> : BaseQueryableVisitor<T>
    {
        public TakeQueryableVisitor(IQueryable<T> queryable) : base(queryable)
        {

        }

        public override IQueryable<T> Visit(object queryFilter)
        {
            PropertyInfo[] properties;
            if (queryFilter is Pager)
            {
                properties = typeof(Pager).GetProperties(BindingFlagsForRetrieveProperties);
            }
            else
            {
                properties = queryFilter.GetType().GetProperties(BindingFlagsForRetrieveProperties);
            }
            foreach (PropertyInfo property in properties)
            {
                PagerTakeAttribute pageTake = property.GetCustomAttribute<PagerTakeAttribute>();
                if (pageTake != null)
                {
                    int count = GetTakeCount(property.GetValue(queryFilter), pageTake.DefaultTakeCount);
                    IQueryable<T> result = queryable.Take(count);

                    return result;
                }
            }

            return queryable;

        }

        private int GetTakeCount(object propertyValue, int defaultTakeCount)
        {
            if (propertyValue == null)
            {
                return defaultTakeCount;
            }

            if (int.TryParse(propertyValue.ToString(), out int result))
            {
                if (result > 0)
                {
                    return result;

                }
            }
            return defaultTakeCount;
        }
    }

}
