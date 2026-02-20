namespace RpgEngine.Core.Models;

public record BaseStats(
    int Hp,
    int MaxHp,
    int Strength,
    int Defense,
    int Speed,
    int Mana,
    int MaxMana
);

public record LevelUpPower(
    int Level,
    string PowerId
);

public record PlayerClass(
    string Id,
    string Name,
    string Description,
    BaseStats BaseStats,
    List<string> StartingItems,
    List<LevelUpPower> LevelUpPowers
);