namespace RpgEngine.Data.DTOs;

public class DataFilesDto
{
    public string Rooms { get; set; } = string.Empty;
    public string Connections { get; set; } = string.Empty;
    public string Items { get; set; } = string.Empty;
    public string Creatures { get; set; } = string.Empty;
    public string Npcs { get; set; } = string.Empty;
    public string Classes { get; set; } = string.Empty;
    public string Powers { get; set; } = string.Empty;
    public string StatusEffects { get; set; } = string.Empty;
}

public class CampaignDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string StartingRoom { get; set; } = string.Empty;
    public DataFilesDto DataFiles { get; set; } = new();
}