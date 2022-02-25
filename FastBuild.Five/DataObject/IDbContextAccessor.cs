
namespace FastBuild.Five.DataAccess
{
    using Microsoft.EntityFrameworkCore;
    /// <summary>
    /// 数据库上下文寄存器
    /// </summary>
    public interface IDbContextAccessor
    {
        /// <summary>
        /// 数据库上下文参数
        /// </summary>
        DbContextOptions dbContextOptions { get; }

        /// <summary>
        /// 数据库上下文
        /// </summary>
        DbContext DbContext { get; }
    }

}
