namespace Doorfail.Core.Data
{
    public abstract class EnumEntity : Entity<int>
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}