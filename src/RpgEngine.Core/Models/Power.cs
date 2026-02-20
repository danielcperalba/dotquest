namespace RpgEngine.Core.Models;

public record PowerEffect(
    string Type,
    string? Target = null,
    int? Value = null,
    string? StatusEffectId = null,
    string? DamageDice = null
);

public record Power(
    string Id,
    string Name,
    string Description,
    string Type,
    int ManaCost,
    int Cooldown,
    List<PowerEffect> Effects,
    string? Condition = null
);