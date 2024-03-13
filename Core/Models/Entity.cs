using System.ComponentModel.DataAnnotations;
using System.IO.Compression;
using System.Text;
using System.Text.Json;

namespace Doorfail.Core.Models;

public interface IEntity<Key> where Key : notnull
{
    Key Id { get; set; }
}

public class Entity<Key> :IEquatable<Entity<Key>>, IEntity<Key> where Key : notnull
{
    [Key]
    public Key Id { get; set; }

    public bool Equals(Entity<Key> other) => Id.Equals(other);

    public override bool Equals(object obj) => obj is Entity<Key> entity && entity.Id.Equals(entity.Id);

    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(Entity<Key> left, Entity<Key> right) => left?.Equals(right) ?? right is null;

    public static bool operator !=(Entity<Key> left, Entity<Key> right) => !(left == right);

    public override string ToString() => $"{GetType().Name} {Id}";

    public static byte[] Compress<T>(T entity)
    {
        var jsonData = JsonSerializer.Serialize(entity);

        // Convert the JSON data to bytes
        byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonData);

        using var memoryStream = new MemoryStream();
        using var deflateStream = new DeflateStream(memoryStream, CompressionMode.Compress, leaveOpen: true);

        // Write the JSON bytes to the DeflateStream
        deflateStream.Write(jsonBytes, 0, jsonBytes.Length);
        deflateStream.Flush(); // Ensure all data is flushed to the underlying stream

        // Get the compressed data from the memory stream
        byte[] compressedData = memoryStream.ToArray();

        return compressedData;
    }

    public static T Decompress<T>(byte[] compressedData)
        where T : Entity<Key>
    {
        // Decompress the compressed data
        using var memoryStream = new MemoryStream(compressedData);
        using var deflateStream = new DeflateStream(memoryStream, CompressionMode.Decompress);
        using var reader = new StreamReader(deflateStream, Encoding.UTF8);

        string jsonData = reader.ReadToEnd();
        return JsonSerializer.Deserialize<T>(jsonData);
    }
}

public interface IOneToOne<Key> where Key : notnull
{
    Key? RelatedId { get; set; }
}

public class OneToOneEntity<Key> :Entity<Key>, IOneToOne<Key>
    where Key : notnull
{
    public Key? RelatedId { get; set; }
}

public interface IOneToMany<Key> where Key : notnull
{
    List<Key> RelatedIds { get; set; }
}

public class OneToManyEntity<Key> :Entity<Key>, IOneToMany<Key>
    where Key : notnull
{
    public List<Key> RelatedIds { get; set; } = [];
}