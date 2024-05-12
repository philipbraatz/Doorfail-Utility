using System.Reflection;
using System.Resources;
using Doorfail.Core.Models;

namespace Doorfail.Core.Licensing;
public class LicenseException :Exception
{
    private static readonly ResourceManager resourceManager = new($"{Assembly.GetExecutingAssembly().GetName().Name}.Resources.Licensing.Strings", Assembly.GetExecutingAssembly());

    public LicenseException(License license) : base(GenerateMessage(license))
    { }
    public LicenseException(License license, Exception inner) : base(GenerateMessage(license), inner)
    { }
    public LicenseException(License license, string expectedProgram) : base(GenerateMessage(license, expectedProgam: expectedProgram))
    { }

    public LicenseException(License license, Exception inner, bool isUnhandled) : base(GenerateMessage(license, isUnhandled ? inner : null), inner)
    { }

    private static string GenerateMessage(License license, Exception? exception = null, string? expectedProgam = null)
    {
        if(!string.IsNullOrEmpty(license.Id) && string.IsNullOrEmpty(license.Name))
            return $"{resourceManager.GetString("InvalidLicense")}: {license.Id}";
        if(license.ExpirationDate < DateOnly.FromDateTime(DateTimeOffset.Now.Date))
            return string.Format(resourceManager.GetString("ExpiredLicense")!, license.ExpirationDate.ToString("yyyy-MM-dd"), license.ContactInfo);
        if(license.Program != expectedProgam)
            return string.Format(resourceManager.GetString("InvalidProgram")!, license.Program);

        if(exception is not null)
        {
            LicensedProgramError error = new()
            {
                License = license,
                Name = exception.Message +  Environment.NewLine+  exception.InnerException?.Message,
                Description = Environment.NewLine + exception.StackTrace,
                CreatedOn = license.CreatedOn,
                UpdatedOn = DateTimeOffset.Now
            };

            return $@"~ {string.Format(resourceManager.GetString("UnexpectedError")!, license.ContactInfo)} ~
Error: {Convert.ToBase64String(Entity<string>.Compress(error))}";
        }

        return license.ToString();
    }

}