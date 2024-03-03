using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doorfail.Core.Licensing;
public class LicenseEvent :License
{
    public DateTimeOffset LogDate { get; set; }
    public string Message { get; set; }
    public int UsageCount { get; set; }
}
