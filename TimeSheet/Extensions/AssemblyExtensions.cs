using System.Reflection;

namespace TimeSheet.Extensions;

public static class AssemblyExtensions {
    public static IEnumerable<Type> GetSubClasseTypes<TAbstact>(this Assembly assembly) where TAbstact : class {
        var types = assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false } && t.IsSubclassOf(typeof(TAbstact)));

        return types;
    }

    public static IEnumerable<Type> GetInterfaceTypes<TInterface>(this Assembly assembly) where TInterface : class {
        var types = assembly.GetTypes()
            .Where(t => typeof(TInterface).IsAssignableFrom(t) && t is { IsInterface: false, IsAbstract: false });

        return types;
    }
}