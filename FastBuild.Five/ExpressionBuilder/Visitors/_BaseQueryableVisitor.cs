

namespace FastBuild.Five.ExpressionBuilder
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    internal abstract class BaseQueryableVisitor<T>
    {

        protected readonly BindingFlags BindingFlagsForRetrieveProperties =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty;

        protected IQueryable<T> queryable { get; set; }
        protected BaseQueryableVisitor(IQueryable<T> queryable)
        {
            this.queryable = queryable;
        }

        public abstract IQueryable<T> Visit(object queryFilter);


        /// <summary>
        /// 检查是否匿名类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected bool CheckIfAnonymousType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
                && type.IsGenericType && type.Name.Contains("AnonymousType")
                && (type.Name.StartsWith("<>"))
                && (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;
        }
    }

}
