namespace Doorfail.Core.Data;

public interface IEntity<TId>
{
    bool Active { get; set; }
    TId CreatedBy { get; set; }
    DateTime CreatedDate { get; set; }
    TId ID { get; set; }
    TId UpdatedBy { get; set; }
    DateTime UpdatedDate { get; set; }

    string ToString();
}