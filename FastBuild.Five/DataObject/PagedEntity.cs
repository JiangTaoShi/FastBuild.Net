


namespace FastBuild.Five.DataObject
{
    using System.Collections.Generic;

    public interface IPaged<TEntity>
    {
        long Total { get; set; }

        IList<TEntity> Rows { get; set; }

    }


    public class Paged<TEntity> : IPaged<TEntity>
    {
        public Paged()
        {
            Total = -1;
            Rows = new List<TEntity>();
        }
        public long Total { get; set; }
        public IList<TEntity> Rows { get; set; }
    }
}
