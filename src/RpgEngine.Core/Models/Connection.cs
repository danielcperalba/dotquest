namespace RpgEngine.Core.Models;

public record Connection(
    string From,
    string To,
    string Direction,
    string? Condition
);