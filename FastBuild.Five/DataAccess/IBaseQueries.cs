using FastBuild.Net.Five.DataObject;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FastBuild.Net.Five.DataAccess
{
    public interface IBaseQueries<TEntityRoot, TDbContext>
        where TEntityRoot : class, IAggregateRoot, new()
        where TDbContext : DbContext
    {
        bool Exist<TEntity>(object filter) where TEntity : class;
        /// <summary>
        /// 根据主键获取数据
        /// </summary>
        /// <param name="id">The identifier.</param>        
        TEntity Find<TEntity>(object rowKey) where TEntity : class;

        ValueTask<TEntity> FindAsync<TEntity>(object rowKey) where TEntity : class;
        /// <summary>
        /// 根据TEntity 获取列表数据
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="filter"></param>
        /// <returns></returns>
        List<TEntity> GetList<TEntity>(object filter) where TEntity : class;
        Task<List<TEntity>> GetListAsync<TEntity>(object filter) where TEntity : class;
        /// <summary>
        /// 根据TEntity 获取分页数据
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="filter"></param>
        /// <returns></returns>
        IPaged<TEntity> GetPagedList<TEntity>(object filter) where TEntity : class;

        Task<IPaged<TEntity>> GetPagedListAsync<TEntity>(object filter) where TEntity : class;
    }
}
