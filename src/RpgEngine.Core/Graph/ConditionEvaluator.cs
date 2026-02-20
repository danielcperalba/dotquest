namespace RpgEngine.Core.Graph;

using RpgEngine.Core.Models;

public static class ConditionEvaluator
{
    public static bool Check(string? condition, GameState state)
    {
        if (string.IsNullOrWhiteSpace(condition))
            return true;

        var negated = condition.StartsWith('!');
        var expr = negated ? condition[1..] : condition;

        var result = Evaluate(expr, state);
        return negated ? !result : result;
    }

    private static bool Evaluate(string expr, GameState state)
    {
        var parts = expr.Split(':');

        return parts[0] switch
        {
            "has_item" => parts.Length >= 2
                && state.Player.Inventory.Contains(parts[1]),

            "flag" => parts.Length >= 2
                && state.Flags.ContainsKey(parts[1]),

            "level" => parts.Length >= 2
                && int.TryParse(parts[1], out var lvl)
                && state.Player.Level >= lvl,

            "stat" => parts.Length >= 3
                && int.TryParse(parts[2], out var minVal)
                && GetStat(parts[1], state.Player) >= minVal,

            _ => false
        };
    }

    private static int GetStat(string statName, PlayerState player)
    {
        return statName.ToLowerInvariant() switch
        {
            "strength" => player.Strength,
            "defense" => player.Defense,
            "speed" => player.Speed,
            "hp" => player.Hp,
            "mana" => player.Mana,
            _ => 0
        };
    }
}