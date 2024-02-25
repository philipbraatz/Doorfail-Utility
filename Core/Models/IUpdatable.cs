namespace Doorfail.Core.Entities;

public interface IUpdatable
{
    public DateTimeOffset CreatedOn { get; set; }
    public DateTimeOffset? UpdatedOn { get; set; }
    public bool IsDeactivated { get; set; }
}

public class UpdatableEntity<Key> :Entity<Key>, IUpdatable where Key : notnull
{
    public DateTimeOffset CreatedOn { get; set; }
    public DateTimeOffset? UpdatedOn { get; set; }
    public bool IsDeactivated { get; set; }
}

// BlameUpdatableEntity
public class BlameUpdatableEntity<Key> :Entity<Key>, IBlamable<Key>, IUpdatable where Key : notnull
{
    public Key CreatedBy { get; set; }
    public Key UpdatedBy { get; set; }

    public DateTimeOffset CreatedOn { get; set; }
    public DateTimeOffset? UpdatedOn { get; set; }
    public bool IsDeactivated { get; set; }
}