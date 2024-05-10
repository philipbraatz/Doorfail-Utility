namespace Doorfail.Core.Data
{
    public abstract class ForignEntity<Tid, Fid> : Entity<Tid>
    {
        public abstract Fid ForignKey { get; set; }
    }
}