

namespace FastBuild.Five.DataObject
{
    using System;
    /// <summary>
    /// 数据权限实体接口
    /// </summary>
    public interface IUserCapacity
    {
        string UserName { get; }

        OrganizationTypeEnum OrganizationType { get; }

        Guid? CompanyId { get; set; }

        Guid? ProjectStageId { get; set; }
    }


}
