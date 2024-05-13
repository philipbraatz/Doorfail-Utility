using Doorfail.Core.Models;

namespace Doorfail.Email.Licensing;
internal class LicensedProgramError :NamedEntityUpdatable<string>
{
    public required License License { get; set; }
}
