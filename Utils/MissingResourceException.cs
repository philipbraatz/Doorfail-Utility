using System.Globalization;
using System.Runtime.Serialization;

namespace Doorfail.Core.Util;
[Serializable]
public class MissingResourceException :Exception
{
    public MissingResourceException()
    {
    }

    public MissingResourceException(string message, CultureInfo culture) : base($"{culture.Name} Missing Key'{message}'")
    {
    }

    public MissingResourceException(string message, Exception innerException) : base(message, innerException)
    {
    }
}