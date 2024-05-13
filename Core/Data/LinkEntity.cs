namespace Doorfail.Core.Data;

public abstract class Link<TId, TEntity1, TEntity2, TId1, TId2>
    where TEntity1 : Entity<TId1>
    where TEntity2 : Entity<TId2>
{
    public TId ID { get; set; }
    public TEntity1 Entity1 { get; set; }
    public TEntity2 Entity2 { get; set; }

    public TId1 Id1 { get => Entity1.ID; set => Entity1.ID = value; }
    public TId2 Id2 { get => Entity2.ID; set => Entity2.ID = value; }

}