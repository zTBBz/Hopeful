using System;
using System.Collections.Generic;

namespace Hopeful.Input;

public static class ActionRegistry
{
    private static readonly Dictionary<string, GameAction> _actions = new();
    private static readonly Dictionary<string, List<GameAction>> _categorizedActions = new();

    public static void Register(GameAction action)
    {
        _actions[action.Id] = action;

        if (!_categorizedActions.ContainsKey(action.Category))
            _categorizedActions[action.Category] = new List<GameAction>();

        _categorizedActions[action.Category].Add(action);
    }

    public static GameAction? GetById(string id) =>
        _actions.TryGetValue(id, out var action) ? action : null;

    public static IReadOnlyList<GameAction> GetByCategory(string category) =>
        _categorizedActions.TryGetValue(category, out var actions) ? actions : Array.Empty<GameAction>();

    public static IReadOnlyDictionary<string, List<GameAction>> GetAllCategories() =>
        _categorizedActions;
}