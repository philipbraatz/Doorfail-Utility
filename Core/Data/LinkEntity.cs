namespace Doorfail.Core.Data;

internal abstract class LinkEntity<TId, FId, F2Id>
{
    public TId ID { get; set; }
    public FId Id1 { get; set; }
    public F2Id Id2 { get; set; }

}
