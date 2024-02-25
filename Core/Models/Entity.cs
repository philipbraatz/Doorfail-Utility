using System.ComponentModel.DataAnnotations;

namespace Doorfail.Core.Entities;

public interface IEntity<Key> where Key : notnull
{
    Key Id { get; set; }
}

public class Entity<Key> :IEquatable<Entity<Key>>, IEntity<Key> where Key : notnull
{
    [Key]
    public Key Id { get; set; }

    public bool Equals(Entity<Key>? other) => Id.Equals(other);

    public override bool Equals(object? obj) => obj is Entity<Key> entity && entity.Id.Equals(entity.Id);

    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(Entity<Key>? left, Entity<Key>? right) => left?.Equals(right) ?? right is null;

    public static bool operator !=(Entity<Key>? left, Entity<Key>? right) => !(left == right);

    public override string ToString() => $"{GetType().Name} {Id}";
}

public interface IOneToOne<Key> where Key : notnull
{
    Key? RelatedId { get; set; }
}

public class OneToOneEntity<Key> : Entity<Key>, IOneToOne<Key>
    where Key : notnull
{
    public Key? RelatedId { get; set; }
}

public interface  IOneToMany<Key> where Key : notnull
{
    List<Key> RelatedIds { get; set; }
}

public class OneToManyEntity<Key> : Entity<Key>, IOneToMany<Key>
    where Key : notnull
{
    public List<Key> RelatedIds { get; set; } = [];
}