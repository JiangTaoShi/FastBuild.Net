


namespace FastBuild.Five.ExpressionBuilder
{
    using FastBuild.Five.DataObject;
    using System.Linq;


    internal class AuthWhereQueryableVisitor<T, UC>
            : WhereQueryableVisitor<T>
        where T : IRowLevelSecurity
        where UC : class, IUserCapacity
    {
        private readonly IQueryable<UC> userCapacity;
        private readonly string userName;

        public AuthWhereQueryableVisitor(IQueryable<T> queryable
            , IQueryable<UC> userCapacity
            , string userName
            ) : base(queryable)
        {
            this.userCapacity = userCapacity;
            this.userName = userName;
        }

        public override IQueryable<T> Visit(object queryFilter)
        {
            IQueryable<T> result = base.Visit(queryFilter);
            // 找到当前用户的所有权限列表
            System.Collections.Generic.List<UC> capacityList = userCapacity.Where(item => item.UserName == userName).ToList();
            result = result.Where(e =>
                            capacityList.Exists(capacity =>
                                    (
                                        // 有集团权限
                                        (capacity.OrganizationType == OrganizationTypeEnum.Group)
                                        // 有地区公司权限
                                        || (capacity.OrganizationType == OrganizationTypeEnum.Company && capacity.CompanyId == e.CompanyId))
                                        // 有项目分期权限
                                        || (capacity.OrganizationType == OrganizationTypeEnum.ProjectStage && capacity.ProjectStageId == e.ProjectStageId)
                                    )
                      );
            return result;

        }

    }

}
