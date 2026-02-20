namespace RpgEngine.Data.Loaders;

using System.Text.Json;
using System.Text.Json.Serialization;
using RpgEngine.Core.Interfaces;
using RpgEngine.Core.Models;
using RpgEngine.Data.DTOs;

public class GameDataLoader : IDataLoader
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public GameData LoadCampaign(string campaignPath)
    {
        var campaignFile = Path.Combine(campaignPath, "campaign.json");
        var campaign = Deserialize<CampaignDto>(campaignFile);

        return new GameData
        {
            CampaignId = campaign.Id,
            Title = campaign.Title,
            StartingRoom = campaign.StartingRoom,
            Rooms = LoadRooms(Path.Combine(campaignPath, campaign.DataFiles.Rooms)),
            Connections = LoadConnections(Path.Combine(campaignPath, campaign.DataFiles.Connections)),
            Items = LoadItems(Path.Combine(campaignPath)),
            Creatures = LoadCreatures(Path.Combine(campaignPath, campaign.DataFiles.Creatures)),
            Classes = LoadClasses(Path.Combine(campaignPath, campaign.DataFiles.Classes)),
            Powers = LoadPowers(Path.Combine(campaignPath, campaign.DataFiles.Powers)),
            StatusEffects = LoadStatusEffects(Path.Combine(campaignPath, campaign.DataFiles.StatusEffects)),
        };
    }

    public List<Room> LoadRooms(string path) => DeserializeList<Room>(path);
    public List<Connection> LoadConnections(string path) => DeserializeList<Connection>(path);
    public List<Item> LoadItems(string path) => DeserializeList<Item>(path);
    public List<Creature> LoadCreatures(string path) => DeserializeList<Creature>(path);
    public List<PlayerClass> LoadClasses(string path) => DeserializeList<PlayerClass>(path);
    public List<Power> LoadPowers(string path) => DeserializeList<Power>(path);
    public List<StatusEffect> LoadStatusEffects(string path) => DeserializeList<StatusEffect>(path);

    private static T Deserialize<T>(string path)
    {
        var json = File.ReadAllBytes(path);
        return JsonSerializer.Deserialize<T>(json, JsonOptions)
            ?? throw new InvalidOperationException($"Falha ao deserializar: {path}");
    }

    private static List<T> DeserializeList<T>(string path)
    {
        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<List<T>>(json, JsonOptions)
            ?? throw new InvalidOperationException($"Falha ao deserializar lista: {path}");
    }
}