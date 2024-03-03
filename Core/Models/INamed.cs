namespace Doorfail.Core.Models;

public interface INamed
{
    #region Properties

    string Name { get; set; }
    string Description { get; set; }

    #endregion Properties
}

public class NamedEntity<Key> :Entity<Key>, INamed
{
    #region Properties

    public string Name { get; set; }
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

public class NamedEntityBlameUpdatable<Key, Blame> :NamedEntity<Key>, IBlamable<Blame>, IUpdatable where Key : notnull
{
    #region Properties

    public Blame CreatedBy { get; set; }
    public Blame UpdatedBy { get; set; }

    public DateTimeOffset CreatedOn { get; set; }
    public DateTimeOffset? UpdatedOn { get; set; }
    public bool IsDeactivated { get; set; }

    #endregion Properties
}