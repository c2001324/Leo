using System.Collections.Generic;
namespace HexMap
{
    /// <summary>
    /// Implementation of Dijkstra pathfinding algorithm.
    /// </summary>
    class DijkstraPathfinding : IPathfinding
    {
        public Dictionary<HexCell, List<HexCell>> findAllPaths(Dictionary<HexCell, Dictionary<HexCell, float>> edges, HexCell originNode)
        {
            IPriorityQueue<HexCell> frontier = new HeapPriorityQueue<HexCell>();
            frontier.Enqueue(originNode, 0);

            Dictionary<HexCell, HexCell> cameFrom = new Dictionary<HexCell, HexCell>();
            cameFrom.Add(originNode, default(HexCell));
            Dictionary<HexCell, float> costSoFar = new Dictionary<HexCell, float>();
            costSoFar.Add(originNode, 0);

            while (frontier.Count != 0)
            {
                var current = frontier.Dequeue();
                var neighbours = GetNeigbours(edges, current);
                foreach (var neighbour in neighbours)
                {
                    var newCost = costSoFar[current] + edges[current][neighbour];
                    if (!costSoFar.ContainsKey(neighbour) || newCost < costSoFar[neighbour])
                    {
                        costSoFar[neighbour] = newCost;
                        cameFrom[neighbour] = current;
                        frontier.Enqueue(neighbour, newCost);
                    }
                }
            }

            Dictionary<HexCell, List<HexCell>> paths = new Dictionary<HexCell, List<HexCell>>();
            foreach (HexCell destination in cameFrom.Keys)
            {
                List<HexCell> path = new List<HexCell>();
                var current = destination;
                while (!current.Equals(originNode))
                {
                    path.Add(current);
                    current = cameFrom[current];
                }
                paths.Add(destination, path);
            }
            return paths;
        }

        public override List<T> FindPath<T>(Dictionary<T, Dictionary<T, float>> edges, T originNode, T destinationNode)
        {
            IPriorityQueue<T> frontier = new HeapPriorityQueue<T>();
            frontier.Enqueue(originNode, 0);

            Dictionary<T, T> cameFrom = new Dictionary<T, T>();
            cameFrom.Add(originNode, default(T));
            Dictionary<T, float> costSoFar = new Dictionary<T, float>();
            costSoFar.Add(originNode, 0);

            while (frontier.Count != 0)
            {
                var current = frontier.Dequeue();
                var neighbours = GetNeigbours(edges, current);
                foreach (var neighbour in neighbours)
                {
                    var newCost = costSoFar[current] + edges[current][neighbour];
                    if (!costSoFar.ContainsKey(neighbour) || newCost < costSoFar[neighbour])
                    {
                        costSoFar[neighbour] = newCost;
                        cameFrom[neighbour] = current;
                        frontier.Enqueue(neighbour, newCost);
                    }
                }
                if (current.Equals(destinationNode)) break;
            }
            List<T> path = new List<T>();
            if (!cameFrom.ContainsKey(destinationNode))
                return path;

            path.Add(destinationNode);
            var temp = destinationNode;

            while (!cameFrom[temp].Equals(originNode))
            {
                var currentPathElement = cameFrom[temp];
                path.Add(currentPathElement);

                temp = currentPathElement;
            }

            return path;
        }
    }
}