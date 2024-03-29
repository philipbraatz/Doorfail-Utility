using System.Globalization;
using System.Resources;
using Doorfail.Core.Util.Extensions;
using static Doorfail.Core.Util.ResourceManager;
using Resource = System.Resources.ResourceManager;

namespace Doorfail.Core.Util;

public class ResourceManager(IEnumerable<Resource> managers, DefaultMissingValue defaultValue = DefaultMissingValue.MissingKey, FailureHandling onFailure = FailureHandling.ReturnNull)
{
    public enum DefaultMissingValue
    {
        Null,
        Empty,
        Key,
        WrappedKey,
        MissingKey,
    }

    private DefaultMissingValue missing = defaultValue;
    private FailureHandling failureMethod = onFailure;
    private ICollection<Resource> resourceManagers = new List<Resource>(managers);

    public string GetString(string key)
    {
        foreach(var resourceManager in resourceManagers)
        {
            string res = resourceManager.GetString(key);
            if(res != null)
            {
                return res;
            }
        }
        return null;
    }

    public string GetString(string key, CultureInfo culture)
    {
        foreach(var resourceManager in resourceManagers)
        {
            string res = resourceManager.GetString(key, culture);
            if(res != null)
            {
                return res;
            }
        }
        return null;
    }

    public string MissingValue(string key)
    {
        var message = $"Key {key} not found in {resourceManagers.Select(rm => rm.BaseName).Join("\n\t")}";
        switch(failureMethod)
        {
            case FailureHandling.ReturnNull:
                return null;

            case FailureHandling.Log:
                Console.WriteLine(message);
                break;

            case FailureHandling.Throw:
                throw new MissingManifestResourceException(message) { Data = { ["Key"] = key } };
            case FailureHandling.ThrowAndLog:
                Console.WriteLine(message);
                throw new MissingManifestResourceException(message) { Data = { ["Key"] = key } };
        };

        return missing switch
        {
            DefaultMissingValue.Null => null,
            DefaultMissingValue.Empty => string.Empty,
            DefaultMissingValue.Key => key,
            DefaultMissingValue.WrappedKey => $"[{key}]",
            DefaultMissingValue.MissingKey => $"[MISSING '{key}']",
        };
    }
}