
namespace FastBuild.Net.Five.DataObject
{
    using System;
    using System.ComponentModel;
    /// <summary>逻辑删除标志</summary> 
    public enum DeleteFlagEnum
    {
        /// <summary>
        /// 只用来表示查询的时候跳过
        /// </summary>
        [Description("全")]
        All = -1,

        /// <summary>有效数据</summary> 
        [Description("正常")]
        Normal = 0,
        /// <summary>已删除数据</summary> 
        [Description("删除")]
        Deleted = 1,
        [Description("删除-多余数据")]
        Invalid = 4,
        /// <summary>隐藏数据</summary> 
        [Description("隐藏数据")]
        Hide = 8,
        /// <summary>待删除数据</summary> 
        [Description("待删除数据")]
        WaitingToDelete = 9,
    }


    [Flags]
    public enum BuildQueryFlagEnum
    {
        List = 1,

        OrderBy = 2,

        SkipAndTake = 4,

        Count = 8
    }

    /// <summary>组织机构类型</summary>
    public enum OrganizationTypeEnum
    {
        /// <summary>集团总部</summary>
        [Description("集团总部")]
        Group = 1,

        /// <summary>地区公司</summary>
        [Description("地区公司")]
        Company = 5,

        /// <summary>项目分期</summary>
        [Description("项目分期")]
        ProjectStage = 9
    }

}
