namespace Doorfail.Core.Entities;

public interface INamed
{
    #region Properties

    string DisplayName { get; set; }
    string Description { get; set; }

    #endregion Properties
}

public class NamedEntity<Key> :Entity<Key>, INamed
{
    #region Properties

    public string DisplayName { get; set; }
    public string Description { get; set; }

    #endregion Properties
}

public class NamedEntityUpdatable<Key> :NamedEntity<Key>, IUpdatable where Key : notnull
{
    #region Properties

    public DateTimeOffset CreatedOn { get; set; }
    public DateTimeOffset? UpdatedOn { get; set; }
    public bool IsDeactivated { get; set; }

    #endregion Properties
}

public class NamedEntityBlamable<Key> :NamedEntity<Key>, IBlamable<Key> where Key : notnull
{
    #region Properties

    public Key CreatedBy { get; set; }
    public Key UpdatedBy { get; set; }

    public DateTimeOffset CreatedOn { get; set; }
    public DateTimeOffset? UpdatedOn { get; set; }

    #endregion Properties
}

public class NamedEntityBlameUpdatable<Key> :NamedEntity<Key>, IBlamable<Key>, IUpdatable where Key : notnull
{
    #region Properties

    public Key CreatedBy { get; set; }
    public Key UpdatedBy { get; set; }

    public DateTimeOffset CreatedOn { get; set; }
    public DateTimeOffset? UpdatedOn { get; set; }
    public bool IsDeactivated { get; set; }

    #endregion Properties
}