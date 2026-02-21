namespace RpgEngine.Core.Tests;

using FluentAssertions;
using RpgEngine.Core.Graph;
using RpgEngine.Core.Models;

public class GraphAlgorithmsTests
{
    private WorldGraph BuildFixtureGraph()
    {
        var graph = new WorldGraph();

        graph.AddRoom(MakeRoom("sala1", "Entrada"));
        graph.AddRoom(MakeRoom("sala2", "Salão"));
        graph.AddRoom(MakeRoom("sala3", "Torre"));
        graph.AddRoom(MakeRoom("sala4", "Caverna"));
        graph.AddRoom(MakeRoom("sala4b", "Caverna Profunda"));
        graph.AddRoom(MakeRoom("sala5", "Jardim"));
        graph.AddRoom(MakeRoom("sala6", "Calabouço"));
        graph.AddRoom(MakeRoom("sala7", "Sala Secreta"));

        graph.AddConnection(new Connection("sala1", "sala2", "norte", null));
        graph.AddConnection(new Connection("sala2", "sala1", "sul", null));
        graph.AddConnection(new Connection("sala2", "sala3", "leste", null));
        graph.AddConnection(new Connection("sala1", "sala4", "oeste", null));
        graph.AddConnection(new Connection("sala4", "sala1", "leste", null));
        graph.AddConnection(new Connection("sala4", "sala4b", "sul", "has_item:corda"));
        graph.AddConnection(new Connection("sala2", "sala5", "norte", null));
        graph.AddConnection(new Connection("sala5", "sala6", "leste", null));

        return graph;
    }

    private static Room MakeRoom(string id, string name)
    {
        return new Room(id, name, $"Descrição de {name}.",
            new List<string>(), new List<string>(), new Dictionary<string, string>());
    }

    // =========== BFS Shortest Path Tests ===========

    [Fact]
    public void Bfs_MesmoNo_DeveRetornarListaUnitaria()
    {
        var graph = BuildFixtureGraph();

        var path = GraphAlgorithms.BfsShortestPath(graph, "sala1", "sala1");

        path.Should().NotBeNull();
        path.Should().Equal("sala1");
    }

    [Fact]
    public void Bfs_Vizinho_DeveRetornarCaminhoDirecto()
    {
        var graph = BuildFixtureGraph();

        var path = GraphAlgorithms.BfsShortestPath(graph, "sala1", "sala2");

        path.Should().NotBeNull();
        path.Should().Equal("sala1", "sala2");
    }

    [Fact]
    public void Bfs_DoisSaltos_DeveRetornarCaminhoCorreto()
    {
        var graph = BuildFixtureGraph();

        var path = GraphAlgorithms.BfsShortestPath(graph, "sala1", "sala3");

        path.Should().NotBeNull();
        path.Should().Equal("sala1", "sala2", "sala3");
    }

    [Fact]
    public void Bfs_TresSaltos_DeveRetornarCaminhoCompleto()
    {
        var graph = BuildFixtureGraph();

        var path = GraphAlgorithms.BfsShortestPath(graph, "sala1", "sala6");

        path.Should().NotBeNull();
        path.Should().Equal("sala1", "sala2", "sala5", "sala6");
    }

    [Fact]
    public void Bfs_SalaInacessivel_DeveRetornarNull()
    {
        var graph = BuildFixtureGraph();

        var path = GraphAlgorithms.BfsShortestPath(graph, "sala1", "sala7");

        path.Should().BeNull();
    }

    [Fact]
    public void Bfs_SalaSemRetorno_DeveRetornarNull()
    {
        var graph = BuildFixtureGraph();

        // sala6 nao tem saidas, nao consegue voltar
        var path = GraphAlgorithms.BfsShortestPath(graph, "sala6", "sala1");

        path.Should().BeNull();
    }

    // =========== DFS Reachable ===========

    [Fact]
    public void Dfs_DaEntrada_DeveAlcancarTodasMenosSalaIsolada()
    {
        var graph = BuildFixtureGraph();

        var reachable = GraphAlgorithms.DfsReachable(graph, "sala1");

        reachable.Should().Contain("sala1");
        reachable.Should().Contain("sala2");
        reachable.Should().Contain("sala3");
        reachable.Should().Contain("sala4");
        reachable.Should().Contain("sala4b");
        reachable.Should().Contain("sala5");
        reachable.Should().Contain("sala6");
        reachable.Should().NotContain("sala7");
    }

    [Fact]
    public void Dfs_DaSalaIsolada_DeveRetornarApenasSi()
    {
        var graph = BuildFixtureGraph();

        var reachable = GraphAlgorithms.DfsReachable(graph, "sala7");

        reachable.Should().HaveCount(1);
        reachable.Should().Contain("sala7");
    }

    [Fact]
    public void Dfs_DaSalaSemSaida_DeveRetornarApenasSi()
    {
        var graph = BuildFixtureGraph();

        var reachable = GraphAlgorithms.DfsReachable(graph, "sala6");

        reachable.Should().HaveCount(1);
        reachable.Should().Contain("sala6");
    }

    // =========== DFS Condicional ===========

    [Fact]
    public void BfsCondicional_SemItens_NaoDeveAlcancarSalaComCondicao()
    {
        var graph = BuildFixtureGraph();
        var state = new GameState();

        var reachable = GraphAlgorithms.BfsConditionalReachable(graph, "sala1", state);

        reachable.Should().NotContain("sala4b");
    }

    [Fact]
    public void BfsCondicional_ComItem_DeveAlcancarSalaComCondicao()
    {
        var graph = BuildFixtureGraph();
        var state = new GameState();
        state.Player.Inventory.Add("corda");

        var reachable = GraphAlgorithms.BfsConditionalReachable(graph, "sala1", state);

        reachable.Should().Contain("sala4b");
    }

    [Fact]
    public void BfsCondicional_SemItem_DeveAlcancarSalasNormais()
    {
        var graph = BuildFixtureGraph();
        var state = new GameState();

        var reachable = GraphAlgorithms.BfsConditionalReachable(graph, "sala1", state);

        reachable.Should().Contain("sala1");
        reachable.Should().Contain("sala2");
        reachable.Should().Contain("sala3");
        reachable.Should().Contain("sala4");
        reachable.Should().Contain("sala5");
        reachable.Should().Contain("sala6");
    }

    // =========== Componentes Conexos  ===========

    [Fact]
    public void ComponentesConexos_DeveDetectarSalaIsolada()
    {
        var graph = BuildFixtureGraph();

        var components = GraphAlgorithms.FindConnectedComponents(graph);

        components.Should().HaveCount(2);

        var salaSecreta = components.First(c => c.Contains("sala7"));
        salaSecreta.Should().HaveCount(1);

        var principal = components.First(c => c.Contains("sala1"));
        principal.Should().HaveCount(7);
    }

    // =========== Detecção de Ciclos  ===========

    [Fact]
    public void HasCycle_ComRetorno_DeveRetornarTrue()
    {
        var graph = BuildFixtureGraph();

        GraphAlgorithms.HasCycle(graph, "sala1", "sala2").Should().BeTrue();
    }

    [Fact]
    public void HasCycle_SemRetorno_DeveRetornarFalse()
    {
        var graph = BuildFixtureGraph();

        GraphAlgorithms.HasCycle(graph, "sala1", "sala6").Should().BeFalse();
    }

    [Fact]
    public void HasCycle_SalaInacessivel_DeveRetornarFalse()
    {
        var graph = BuildFixtureGraph();

        GraphAlgorithms.HasCycle(graph, "sala1", "sala7").Should().BeFalse();
    }
}