namespace Doorfail.Core.Models;

public abstract class ForeignEntity<Tid, Fid> :Entity<Tid>
{
    public Fid ForeignId { get; set; }
}