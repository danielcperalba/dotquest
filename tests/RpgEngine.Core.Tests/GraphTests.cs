namespace RpgEngine.Core.Tests;

using FluentAssertions;
using RpgEngine.Core.Graph;
using RpgEngine.Core.Models;

public class WorldGraphTests
{
    private WorldGraph BuildSimpleGraph()
    {
        var graph = new WorldGraph();

        graph.AddRoom(new Room("sala1", "Entrada", "Uma entrada escura.",
            new List<string>(), new List<string>(), new Dictionary<string, string>()));
        graph.AddRoom(new Room("sala2", "Corredor", "Um corredor longo.",
            new List<string>(), new List<string>(), new Dictionary<string, string>()));
        graph.AddRoom(new Room("sala3", "Tesouro", "Sala do tesouro.",
            new List<string>(), new List<string>(), new Dictionary<string, string>()));

        graph.AddConnection(new Connection("sala1", "sala2", "norte", null));
        graph.AddConnection(new Connection("sala2", "sala1", "sul", null));
        graph.AddConnection(new Connection("sala2", "sala3", "leste", "has_item:chave"));

        return graph;
    }

    [Fact]
    public void AddRoom_DeveArmazenarSala()
    {
        var graph = new WorldGraph();
        var room = new Room("r1", "Sala", "Desc",
            new List<string>(), new List<string>(), new Dictionary<string, string>());

        graph.AddRoom(room);

        graph.RoomCount.Should().Be(1);
        graph.GetRoom("r1").Should().NotBeNull();
        graph.GetRoom("r1")!.Name.Should().Be("Sala");
    }

    [Fact]
    public void GetRoom_IdInexistente_DeveRetornarNull()
    {
        var graph = new WorldGraph();

        graph.GetRoom("inexistente").Should().BeNull();
    }

    [Fact]
    public void GetExits_DeteRetornarConexoesDaSala()
    {
        var graph = BuildSimpleGraph();

        var exits = graph.GetExits("sala2");

        exits.Should().HaveCount(2);
        exits.Select(c => c.Direction).Should().Contain("sul");
        exits.Select(c => c.Direction).Should().Contain("leste");
    }

    [Fact]
    public void GetExits_SalaSemSaidas_DeveRetornarListaVazia()
    {
        var graph = BuildSimpleGraph();

        graph.GetExits("sala3").Should().BeEmpty();
    }

    [Fact]
    public void Navigate_DirecaoValida_DeveRetornarSalaDestino()
    {
        var graph = BuildSimpleGraph();
        var state = new GameState { CurrentRoomId = "sala1" };

        var result = graph.Navigate("sala1", "norte", state);

        result.Should().NotBeNull();
        result!.Id.Should().Be("sala2");
    }

    [Fact]
    public void Navigate_DirecaoInexistente_DeveRetornarNull()
    {
        var graph = BuildSimpleGraph();
        var state = new GameState();

        graph.Navigate("sala1", "oeste", state).Should().BeNull();
    }

    [Fact]
    public void Navigate_CondicaoNaoAtendida_DeveRetornarNull()
    {
        var graph = BuildSimpleGraph();
        var state = new GameState();

        graph.Navigate("sala2", "leste", state).Should().BeNull();
    }

    [Fact]
    public void Navigate_CondicaoAtendida_DeveRetornar()
    {
        var graph = BuildSimpleGraph();
        var state = new GameState();
        state.Player.Inventory.Add("chave");

        var result = graph.Navigate("sala2", "leste", state);

        result.Should().NotBeNull();
        result!.Id.Should().Be("sala3");
    }

    [Fact]
    public void GetAvailableExits_DeveRespeitarCondicoes()
    {
        var graph = BuildSimpleGraph();
        var state = new GameState();

        var exits = graph.GetAvailableExits("sala2", state);
        exits.Should().HaveCount(1);
        exits[0].Direction.Should().Be("sul");

        state.Player.Inventory.Add("chave");

        var exitsComChave = graph.GetAvailableExits("sala2", state);
        exitsComChave.Should().HaveCount(2);
    }

    [Fact]
    public void ConnectionCount_DeveContarTodasConexoes()
    {
        var graph = BuildSimpleGraph();

        graph.ConnectionCount.Should().Be(3);
    }
}