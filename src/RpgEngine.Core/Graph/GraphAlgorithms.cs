namespace RpgEngine.Core.Graph;

using RpgEngine.Core.Models;

public static class GraphAlgorithms
{
    /// BFS - Retorna o caminho mais curto entre duas salas.
    /// Ignora condicoes (mapa "ideal"). Retorna null se nao houver caminho.

    public static List<string>? BfsShortestPath(WorldGraph graph, string startId, string goalId)
    {
        if (startId == goalId)
            return new List<string> { startId };

        var visited = new HashSet<string> { startId };
        var queue = new Queue<List<string>>();
        queue.Enqueue(new List<string> { startId });

        while (queue.Count > 0)
        {
            var path = queue.Dequeue();
            var current = path[^1];

            foreach (var exit in graph.GetExits(current))
            {
                if (visited.Contains(exit.To))
                    continue;

                var newPath = new List<string>(path) { exit.To };

                if (exit.To == goalId)
                    return newPath;

                visited.Add(exit.To);
                queue.Enqueue(newPath);
            }
        }

        return null;
    }

    /// DFS - Retorna todas as salas alcancaveis a partir de uma sala.
    /// Ignora condicoes.

    public static HashSet<string> DfsReachable(WorldGraph graph, string startId)
    {
        var visited = new HashSet<string>();
        var stack = new Stack<string>();
        stack.Push(startId);

        while (stack.Count > 0)
        {
            var current = stack.Pop();

            if (!visited.Add(current))
                continue;

            foreach (var exit in graph.GetExits(current))
            {
                if (!visited.Contains(exit.To))
                    stack.Push(exit.To);
            }
        }

        return visited;
    }

    /// BFS Condicional - Retorna salas acessiveis dado o estado atual do jogador.
    /// Respeita condicoes das conexoes.

    public static HashSet<string> BfsConditionalReachable(WorldGraph graph, string startId, GameState state)
    {
        var visited = new HashSet<string> { startId };
        var queue = new Queue<string>();
        queue.Enqueue(startId);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            foreach (var exit in graph.GetAvailableExits(current, state))
            {
                if (visited.Add(exit.To))
                    queue.Enqueue(exit.To);
            }
        }

        return visited;
    }

    /// Componentes Conexos - Agrupa salas em componentes (ignora direcao das arestas).
    /// Util para detectar salas isoladas (bug de design).

    public static List<HashSet<string>> FindConnectedComponents(WorldGraph graph)
    {
        var allRooms = graph.GetAllRoomsIds();
        var visited = new HashSet<string>();
        var components = new List<HashSet<string>>();

        // Monta grafo nao-dirigido para analise de componentes
        var undirected = new Dictionary<string, HashSet<string>>();
        foreach (var roomId in allRooms)
        {
            if (!undirected.ContainsKey(roomId))
                undirected[roomId] = new HashSet<string>();

            foreach (var exit in graph.GetExits(roomId))
            {
                if (!undirected.ContainsKey(exit.To))
                    undirected[exit.To] = new HashSet<string>();

                undirected[roomId].Add(exit.To);
                undirected[exit.To].Add(roomId);
            }
        }

        foreach (var roomId in allRooms)
        {
            if (visited.Contains(roomId))
                continue;

            var component = new HashSet<string>();
            var queue = new Queue<string>();
            queue.Enqueue(roomId);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                if (!component.Add(current))
                    continue;

                visited.Add(current);

                if (undirected.TryGetValue(current, out var neighbors))
                {
                    foreach (var neighbor in neighbors)
                    {
                        if (!component.Contains(neighbor))
                            queue.Enqueue(neighbor);
                    }
                }
            }

            components.Add(component);
        }

        return components;
    }

    /// Deteccao de Ciclos - Verifica se existe rota de volta de goalId para startId.

    public static bool HasCycle(WorldGraph graph, string startId, string goalId)
    {
        var pathToGoal = BfsShortestPath(graph, startId, goalId);
        if (pathToGoal == null)
            return false;

        var pathBack = BfsShortestPath(graph, goalId, startId);
        return pathBack != null;
    }
}