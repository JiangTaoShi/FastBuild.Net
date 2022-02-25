

namespace FastBuild.Five.DataObject
{
    using FastBuild.Five.Filter;
    public class Pager
    {
        [SpecialOperation]
        public int PageSize { get; set; } = 10000;


        /// <summary>
        /// 分页序号， 基于1开始
        /// </summary>
        [SpecialOperation]
        public int PageNumber { get; set; } = 1;

        [PagerTake(10000)]
        public int TakeCount { get => PageSize; set => PageSize = value; }

        [PagerSkip()]
        public int SkipCount
        {
            get
            {
                if (PageNumber > 1)
                {
                    return (PageNumber - 1) * PageSize;
                }
                else
                {
                    return 0;
                }
            }
        }
    }
    public class PagerAndSortBySortNo : Pager
    {

        [OrderBy("SortNo", DefaultAscending = true)]
        public bool? SortNo { get; set; } = true;
    }

    public class PagerAndSortBySortIndex : Pager
    {

        [OrderBy("SortIndex", DefaultAscending = true)]
        public bool? Sort_SortIndex { get; set; } = true;
    }

    public class PagerAndSortByProjectStageName : Pager
    {

        [OrderBy("ProjectStageName", DefaultAscending = true)]
        public bool? Sort_ProjectStageName { get; set; } = true;
    }

    public class PagerAndSortByCreatedTime : Pager
    {

        [OrderBy("CreatedTime", DefaultAscending = false)]
        public bool? Sort_CreatedTime { get; set; }
    }

    public class PagerAndSortByModifiedTime : Pager
    {

        [OrderBy("ModifiedTime", DefaultAscending = false)]
        public bool? Sort_ModifiedTime { get; set; }
    }
}