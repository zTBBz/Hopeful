using Hopeful.Utilities;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Hopeful.Input;

public class InputBindings : Singleton<InputBindings>
{
    private readonly Dictionary<string, KeyBind> _bindings = new();
    private readonly Dictionary<string, KeyBind> _defaultBindings = new();

    public InputBindings()
    {
        InitializeDefaultBindings();
    }

    private void InitializeDefaultBindings()
    {
        SetDefaultBinding(GameAction.UI.Confirm, new KeyBind(Keys.Enter));
        SetDefaultBinding(GameAction.UI.Cancel, new KeyBind(Keys.Escape));
    }

    public void ResetToDefaults()
    {
        _bindings.Clear();
        foreach (var kvp in _defaultBindings)
            _bindings[kvp.Key] = kvp.Value;
    }

    private void SetDefaultBinding(GameAction action, KeyBind bind)
    {
        _defaultBindings[action.Id] = bind;
        if (!_bindings.ContainsKey(action.Id))
            _bindings[action.Id] = bind;
    }

    public void SetBinding(GameAction action, KeyBind bind)
    {
        _bindings[action.Id] = bind;
        SaveBindings();
    }

    public KeyBind GetBinding(GameAction action)
        => _bindings.TryGetValue(action.Id, out var bind) ? bind : _defaultBindings[action.Id];

    private void SaveBindings()
    {
        // TODO: Save to JSON
    }

    private void LoadBindings()
    {
        // TODO: Load from JSON
    }
}