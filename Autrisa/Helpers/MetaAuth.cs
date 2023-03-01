namespace Autrisa.Helpers
{
    public class MetaAuth
    {
        public MetaAuth()
        {
            Created = DateTime.Now;
            Modified = null;
            ModifiedUserId = null;
            Removed = null;
            RemovedUserId = null;
        }

        public DateTime Created { get; set; }
        public int CreatedUserId { get; set; }

        public Nullable<DateTime> Modified { get; set; }
        public Nullable<int> ModifiedUserId { get; set; }
        public Nullable<DateTime> Removed { get; set; }
        public Nullable<int> RemovedUserId { get; set; }
    }
}