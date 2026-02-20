namespace RpgEngine.Core.Interfaces;

using RpgEngine.Core.Models;

public interface IDataLoader
{
    List<Room> LoadRooms(string path);
    List<Connection> LoadConnections(string path);
    List<Item> LoadItems(string path);
    List<Creature> LoadCreatures(string path);
    List<PlayerClass> LoadClasses(string path);
    List<Power> LoadPowers(string path);
    List<StatusEffect> LoadStatusEffects(string path);
}