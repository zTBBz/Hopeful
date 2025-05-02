namespace Hopeful.Input;

public class GameAction
{
    public string Id { get; }
    public string Category { get; }

    private GameAction(string id, string category)
    {
        Id = id;
        Category = category;
    }

    public static class UI
    {
        public static readonly GameAction Confirm = Create("ui.confirm", "UI");
        public static readonly GameAction Cancel = Create("ui.cancel", "UI");
        public static readonly GameAction Menu = Create("ui.menu", "UI");
    }

    public static GameAction Create(string id, string category)
    {
        var action = new GameAction(id, category);
        ActionRegistry.Register(action);
        return action;
    }

    public override int GetHashCode() => Id.GetHashCode();
    public override bool Equals(object? obj) => obj is GameAction other && Id == other.Id;
}