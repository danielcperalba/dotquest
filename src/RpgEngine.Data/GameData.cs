namespace RpgEngine.Data;

using RpgEngine.Core.Models;

public class GameData
{
    public string CampaignId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string StartingRoom { get; set; } = string.Empty;
    public List<Room> Rooms { get; set; } = new();
    public List<Connection> Connections { get; set; } = new();
    public List<Item> Items { get; set; } = new();
    public List<Creature> Creatures { get; set; } = new();
    public List<PlayerClass> Classes { get; set; } = new();
    public List<Power> Powers { get; set; } = new();
    public List<StatusEffect> StatusEffects { get; set; } = new();

}