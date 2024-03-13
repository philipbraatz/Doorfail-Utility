namespace Doorfail.Core.Licensing;
public class LicenseEvent :License
{
    public DateTimeOffset LogDate { get; set; }
    public string Message { get; set; }
    public int UsageCount { get; set; }
}
