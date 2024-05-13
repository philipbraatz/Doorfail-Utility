namespace Doorfail.Core.Data;

public abstract class ForeignEntity<Tid, Fid> : Entity<Tid>
{
    public Fid ForeignId { get; set; }
}
