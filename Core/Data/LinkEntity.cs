namespace Doorfail.Core.Data
{
    public abstract class Link<TId, FId, F2Id>
    {
        public TId ID { get; set; }
        public FId Id1 { get; set; }
        public F2Id Id2 { get; set; }

    }

    public abstract class LinkEntity<TId, F1, F2, F1id, F2id> : Link<TId, F1, F2>
        where F1 : Entity<F1id>
        where F2 : Entity<F2id>
    {

    }
}
