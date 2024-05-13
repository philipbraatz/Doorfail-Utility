using System.Reflection;

namespace Doorfail.Core.Util;

// Warning: This is a bad idea, only use this when the only data you can pass is through generic types and you don't care about what type the generic instance is.
public class UniqueTypeRegistry<TValue> :Dictionary<Type, TValue>
{
    private readonly Type[] AssemblyTypes;

    public UniqueTypeRegistry(Assembly? assembly = null)
    {
        assembly ??= Assembly.GetCallingAssembly();

        AssemblyTypes = assembly.GetTypes()
        .Where(t => !t.IsAbstract && !t.IsInterface && t.IsClass)
        .ToArray();

        Console.WriteLine($"Max number of unique types available: {AssemblyTypes.Length}");
    }

    public Type Add(TValue value)
    {
        var type = GetNextFreeType();
        this.Add(type, value);

        return type;
    }

    // Yes we are just connecting it to random types.
    public Type GetNextFreeType() => AssemblyTypes.FirstOrDefault(t => !this.ContainsKey(t));
}