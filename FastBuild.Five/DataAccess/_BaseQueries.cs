namespace FastBuild.Five.DataAccess
{
    using FastBuild.Five.DataObject;
    using FastBuild.Five.ExpressionBuilder;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public abstract class BaseQueries<TEntityRoot, TDbContext>
        where TEntityRoot : class, IAggregateRoot, new()
        where TDbContext : DbContext
    {
        private TDbContext _readOnlyContext => GetDbContext();

        protected abstract TDbContext GetDbContext();

        public virtual bool Exist<TEntity>(object filter) where TEntity : class
        {
            if (filter == null)
            {
                filter = new object();
            }
            bool result = BuildQueryable<TEntity>(filter, _readOnlyContext, BuildQueryFlagEnum.Count).Any();
            return result;
        }

        /// <summary>
        /// 根据主键获取数据
        /// </summary>
        /// <param name="id">The identifier.</param>        
        public virtual TEntity Find<TEntity>(object rowKey) where TEntity : class
        {
            return _readOnlyContext.Set<TEntity>()?.Find(rowKey);
        }

        public virtual ValueTask<TEntity> FindAsync<TEntity>(object rowKey) where TEntity : class
        {
            return _readOnlyContext.Set<TEntity>().FindAsync(rowKey);
        }



        /// <summary>
        /// 根据TEntity 获取列表数据
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="filter"></param>
        /// <returns></returns>
        public virtual List<TEntity> GetList<TEntity>(object filter) where TEntity : class
        {
            if (filter == null)
            {
                filter = new object();
            }

            IQueryable<TEntity> queryable = BuildQueryable<TEntity>(filter,
                    _readOnlyContext,
                    BuildQueryFlagEnum.List | BuildQueryFlagEnum.OrderBy);

            return queryable.ToList();
        }


        public virtual Task<List<TEntity>> GetListAsync<TEntity>(object filter) where TEntity : class
        {
            if (filter == null)
            {
                filter = new object();
            }

            IQueryable<TEntity> queryable = BuildQueryable<TEntity>(filter,
                    _readOnlyContext,
                    BuildQueryFlagEnum.List | BuildQueryFlagEnum.OrderBy);

            return queryable.ToListAsync(); ;
        }

        /// <summary>
        /// 根据TEntity 获取分页数据
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="filter"></param>
        /// <returns></returns>
        public virtual IPaged<TEntity> GetPagedList<TEntity>(object filter) where TEntity : class
        {
            if (filter == null)
            {
                filter = new object();
            }
            IPaged<TEntity> result = new Paged<TEntity>();

            IQueryable<TEntity> queryable = BuildQueryable<TEntity>(filter, _readOnlyContext, ListAndPagerComboFlag);
            result.Rows = queryable.ToList();

            IQueryable<TEntity> queryableForCount = BuildQueryable<TEntity>(filter, _readOnlyContext, BuildQueryFlagEnum.Count);
            result.Total = queryableForCount.Count();

            return result;
        }

        public virtual async Task<IPaged<TEntity>> GetPagedListAsync<TEntity>(object filter) where TEntity : class
        {
            if (filter == null)
            {
                filter = new object();
            }
            IPaged<TEntity> result = new Paged<TEntity>();

            IQueryable<TEntity> queryable = BuildQueryable<TEntity>(filter, _readOnlyContext, ListAndPagerComboFlag);
            result.Rows = await queryable.ToListAsync().ConfigureAwait(false);

            IQueryable<TEntity> queryableForCount = BuildQueryable<TEntity>(filter, _readOnlyContext, BuildQueryFlagEnum.Count);
            result.Total = await queryableForCount.CountAsync().ConfigureAwait(false);

            return result;
        }

        /// <summary>
        /// 构建EF查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter"></param>
        /// <param name="context"></param>
        /// <param name="buildQueryFlag">查询的步骤Flag, 参考BuildQueryFlagEnum</param>
        /// <returns></returns>
        protected virtual IQueryable<T> BuildQueryable<T>(
            object filter,
            Microsoft.EntityFrameworkCore.DbContext context,
            BuildQueryFlagEnum buildQueryFlag = BuildQueryFlagEnum.List)
            where T : class
        {
            if (!buildQueryFlag.HasFlag(BuildQueryFlagEnum.List)
                && !buildQueryFlag.HasFlag(BuildQueryFlagEnum.Count))
            {
                throw new ArgumentException("必须包含List/Count其中之一。");
            }

            if (filter == null)
            {
                filter = new object();
            }


            IQueryable<T> queryable = _readOnlyContext.Set<T>().AsNoTracking().AsQueryable();
            queryable = new WhereQueryableVisitor<T>(queryable).Visit(filter);

            if (buildQueryFlag.HasFlag(BuildQueryFlagEnum.List)
              && buildQueryFlag.HasFlag(BuildQueryFlagEnum.OrderBy))
            {
                queryable = new OrderByQueryableVisitor<T>(queryable).Visit(filter);
            }

            if (buildQueryFlag.HasFlag(BuildQueryFlagEnum.List)
                && buildQueryFlag.HasFlag(BuildQueryFlagEnum.SkipAndTake))
            {
                queryable = new SkipQueryableVisitor<T>(queryable).Visit(filter);
                queryable = new TakeQueryableVisitor<T>(queryable).Visit(filter);
            }

            return queryable;
        }

        /// <summary>
        /// 返回列表并带排序、分页， 
        /// </summary>
        protected readonly BuildQueryFlagEnum ListAndPagerComboFlag =
            BuildQueryFlagEnum.List | BuildQueryFlagEnum.OrderBy | BuildQueryFlagEnum.SkipAndTake;
    }



}
