

namespace FastBuild.Net.Five.DataObject
{
    using System;


    /// <summary>
    /// 行级数据安全
    /// </summary>
    public interface IRowLevelSecurity
    {
        Guid? CompanyId { get; set; }
        Guid? ProjectStageId { get; set; }
    }
}
