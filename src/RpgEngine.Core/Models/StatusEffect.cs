namespace RpgEngine.Core.Models;

public record TickEffect(
    string Type,
    int Value
);

public record StatusEffect(
    string Id,
    string Name,
    string Description,
    int Duration,
    List<TickEffect> EffectsPerTick,
    Dictionary<string, int>? StatModifiers = null
);
