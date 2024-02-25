namespace Doorfail.Core.Entities;

public interface IBlamable<Key>
{
    public Key CreatedBy { get; set; }
    public Key UpdatedBy { get; set; }

    public DateTimeOffset CreatedOn { get; set; }
    public DateTimeOffset? UpdatedOn { get; set; }
}

public class BlamableEntity<Key> :Entity<Key>, IBlamable<Key> where Key : notnull
{
    public Key CreatedBy { get; set; }
    public Key UpdatedBy { get; set; }

    public DateTimeOffset CreatedOn { get; set; }
    public DateTimeOffset? UpdatedOn { get; set; }
}