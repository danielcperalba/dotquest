namespace RpgEngine.Core.Graph;

using RpgEngine.Core.Models;

public class WorldGraph
{
    private readonly Dictionary<string, Room> _rooms = new();
    private readonly Dictionary<string, List<Connection>> _adjacency = new();

    public void AddRoom(Room room)
    {
        _rooms[room.Id] = room;
        if (!_adjacency.ContainsKey(room.Id))
            _adjacency[room.Id] = new List<Connection>();
    }

    public void AddConnection(Connection connection)
    {
        if (!_adjacency.ContainsKey(connection.From))
            _adjacency[connection.From] = new List<Connection>();

        _adjacency[connection.From].Add(connection);
    }

    public Room? GetRoom(string roomId)
    {
        return _rooms.TryGetValue(roomId, out var room) ? room : null;
    }

    public IReadOnlyList<Connection> GetExits(string roomId)
    {
        return _adjacency.TryGetValue(roomId, out var connections)
            ? connections.AsReadOnly()
            : Array.Empty<Connection>();
    }

    public IReadOnlyCollection<string> GetAllRoomsIds()
    {
        return _rooms.Keys;
    }

    public IReadOnlyList<Connection> GetAvailableExits(string roomId, GameState state)
    {
        return GetExits(roomId)
            .Where(c => ConditionEvaluator.Check(c.Condition, state))
            .ToList()
            .AsReadOnly();
    }

    public Room? Navigate(string fromRoomId, string direction, GameState state)
    {
        var connection = GetExits(fromRoomId)
            .FirstOrDefault(c =>
                c.Direction.Equals(direction, StringComparison.OrdinalIgnoreCase)
                && ConditionEvaluator.Check(c.Condition, state));

        if (connection is null)
            return null;

        return GetRoom(connection.To);
    }

    public int RoomCount => _rooms.Count;

    public int ConnectionCount => _adjacency.Values.Sum(c => c.Count);
}