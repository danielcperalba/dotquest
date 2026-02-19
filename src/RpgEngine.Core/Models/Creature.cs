namespace RpgEngine.Core.Models;

public record CreatureStats(
    int Hp,
    int MaxHp,
    int Strength,
    int Defense,
    int Speed
);

public record CreatureCombat(
    string DamageDice,
    int AttackBonus,
    int DefenseBonus
);

public record LootEntry(
    string ItemId,
    double DropChance
);

public record Creature(
    string Id,
    string Name,
    string Description,
    CreatureStats Stats,
    CreatureCombat Combat,
    List<LootEntry> LootTable,
    int XpReward = 0
);