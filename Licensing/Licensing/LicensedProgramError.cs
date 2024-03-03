using Doorfail.Core.Models;

namespace Doorfail.Core.Licensing;
internal class LicensedProgramError :NamedEntityUpdatable<string>
{
    public required License License { get; set; }
}
