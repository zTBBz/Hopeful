using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Hopeful.Mod;

public class ModLoader
{
    public bool TryLoadModAssembly(string path, out Assembly? assembly)
    {
        assembly = Assembly.LoadFrom(path);
        return assembly != null;
    }

    public bool TryGetBaseMod(Assembly assembly, out BaseMod? mod)
    {
        var modType = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(BaseMod)) && !t.IsAbstract).First();
        mod = null;
        try
        {
            mod = (BaseMod)Activator.CreateInstance(modType)!;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error mod creating {modType.Name}: {ex.Message}");
        }

        return mod != null;
    }
}
