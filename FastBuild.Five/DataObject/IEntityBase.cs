


namespace FastBuild.Five.DataObject
{
    using System;

    /// <summary>
    /// 实体基类接口
    /// </summary>
    public interface IEntityBase
    {
        /// <summary> 创建时间 </summary>
        DateTime? CreatedTime { get; set; }

        /// <summary> 创建人 </summary>
        string Creator { get; set; }

        /// <summary> 删除标记 </summary>
        DeleteFlagEnum? DeleteFlag { get; set; }

        /// <summary> 修改时间 </summary>
        DateTime? ModifiedTime { get; set; }

        /// <summary> 修改人 </summary>
        string Modifier { get; set; }

        /// <summary> 创建日期 </summary>
        DateTime? CreatedDate { get; }

        /// <summary> 修改日期 </summary>
        DateTime? ModifiedDate { get; }

        /// <summary> 创建时间 </summary>
        string DeleteFlagDesc { get; }
    }

}