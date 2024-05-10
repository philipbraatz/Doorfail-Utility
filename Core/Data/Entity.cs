namespace Doorfail.Core.Data
{
    public abstract class Entity<TId>
    {
        public abstract TId ID { get; set; }

        public abstract DateTime CreatedDate { get; set; }
        public abstract DateTime UpdatedDate { get; set; }
        public abstract TId CreatedBy { get; set; }
        public abstract TId UpdatedBy { get; set; }
        public abstract bool Active { get; set; }
    }
}