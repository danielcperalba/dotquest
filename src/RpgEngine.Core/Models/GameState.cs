namespace RpgEngine.Core.Models;

public class ActiveStatusEffect
{
    public string StatusEffectId { get; set; } = string.Empty;
    public int RemainingTurns { get; set; }
}

public class PlayerState
{
    public string Name { get; set; } = string.Empty;
    public string ClassId { get; set; } = string.Empty;
    public int Level { get; set; } = 1;
    public int Xp { get; set; } = 0;
    public int Hp { get; set; }
    public int MaxHp { get; set; }
    public int Mana { get; set; }
    public int MaxMana { get; set; }
    public int Strength { get; set; }
    public int Defense { get; set; }
    public int Speed { get; set; }
    public List<string> Inventory { get; set; } = new();
    public List<string> KnownPowers { get; set; } = new();
    public Dictionary<string, int> PowerCooldowns { get; set; } = new();
    public List<ActiveStatusEffect> StatusEffects { get; set; } = new();
}

public class GameState
{
    public string CurrentRoomId { get; set; } = string.Empty;
    public PlayerState Player { get; set; } = new();
    public Dictionary<string, string> Flags { get; set; } = new();
    public HashSet<string> DefeatedCreatures { get; set; } = new();
    public HashSet<string> VisitedRooms { get; set; } = new();
    public int TurnCount { get; set; } = 0;
}