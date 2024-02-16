using System.ComponentModel;

namespace Doorfail.Core.Shop.Web.Api.Models;

[Flags]
public enum RolePermission
{
    [Description()] None = 0,
    [Description()] Read = 1,
    [Description()] UpdateSelf = 2,
    [Description()] UpdateFlagged = 4,
    [Description()] ReadFlagged = 8,
    [Description()] QueryOthers = 16,
    [Description()] Flag = 32,
    [Description()] All = 63
}
