using System;
using System.Collections.Generic;

namespace Hopeful.Input;

[Service]
public sealed class KeyBindStore : IDisposable
{
    private readonly Dictionary<string, KeyBind> _defaultBinds = [];
    private Dictionary<string, KeyBind> _binds = [];

    public void RegisterDefaultKeyBind(string name, KeyBind keyBind)
    => _defaultBinds[name] = keyBind;

    public void RegisterKeyBind(string name, KeyBind keyBind)
        => _binds[name] = keyBind;

    public bool TryGetKeyBind(string id, out KeyBind keyBind)
        => _binds.TryGetValue(id, out keyBind);

    public KeyBind TryGetKeyBind(string id)
        => _binds.TryGetValue(id, out var bind) ? bind : _defaultBinds[id];

    public void ResetToDefaults()
    {
        _binds.Clear();
        _binds = _defaultBinds;
    }

    public IReadOnlyDictionary<string, KeyBind> GetKeyBinds()
        => _binds;

    public IReadOnlyDictionary<string, KeyBind> GetDefaultKeyBinds()
        => _defaultBinds;

    public void Dispose()
    {
        _binds.Clear();
        GC.SuppressFinalize(this);
    }
}
