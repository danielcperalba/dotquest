namespace RpgEngine.Core.Models;

public record Room(
    string Id,
    string Name,
    string Description,
    List<string> Items,
    List<string> Npcs,
    Dictionary<string, string> Flags
);