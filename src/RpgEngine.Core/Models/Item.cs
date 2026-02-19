namespace RpgEngine.Core.Models;

public record Item(
    string Id,
    string Name,
    string Description,
    string Type,
    Dictionary<string, int>? StatModifiers = null,
    int? Damage = null,
    int? Defense = null,
    bool Consumable = false
);