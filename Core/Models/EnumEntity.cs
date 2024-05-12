namespace Doorfail.Core.Models
{
    public abstract class EnumEntity :Entity<int>
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}