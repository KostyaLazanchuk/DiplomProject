using BusinessLogic.Interface;
using Diplom.Core.Models;

namespace Algorithm.Algorithms
{
    public class AStarAlgorithm
    {
        private readonly ICommonService _commonService;
        private readonly INodeService _nodeService;

        public AStarAlgorithm(ICommonService commonService, INodeService nodeService)
        {
            _commonService = commonService;
            _nodeService = nodeService;
        }

        public async Task<List<Guid>> FindShortestPath(Node start, Node goal)
        {
            var openSet = new HashSet<Node>();
            var closedSet = new HashSet<Node>();
            var cameFrom = new Dictionary<Node, Node>();
            var gScore = new Dictionary<Node, int>();
            var fScore = new Dictionary<Node, int>();

            foreach (var node in await GetNodes())
            {
                gScore[node] = int.MaxValue;
                fScore[node] = int.MaxValue;
            }

            gScore[start] = 0;
            fScore[start] = HeuristicCostEstimate(start, goal);
            openSet.Add(start);

            while (openSet.Any()) 
            {
                var current = openSet.OrderBy(node => fScore[node]).First();

                if (current == goal)
                    return ReconstructPath(cameFrom, current);

                openSet.Remove(current);
                closedSet.Add(current);

                var neighbors = await GetNeighborsAsync(current.Id);
                foreach (var neighbor  in neighbors)
                {
                    if(closedSet.Contains(neighbor)) continue;

                    var tentativeGScore = gScore[current] + await GetDistance(current.Id, neighbor.Id);
                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                    else if (tentativeGScore >= gScore[neighbor])
                        continue;

                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = gScore[neighbor] + HeuristicCostEstimate(neighbor, goal);
                }
            }

            return new List<Guid>();
        }

        private async Task<List<Node>> GetNodes()
        {
            return await _commonService.GetAllNodesWithRelationships();
        }

        private int HeuristicCostEstimate(Node start, Node goal)
        {
            return Math.Abs(start.Position - goal.Position);
        }

        private List<Guid> ReconstructPath(Dictionary<Node, Node> cameFrom, Node current)
        {
            var totalPath = new List<Guid>() { current.Id };
            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                totalPath.Insert(0, current.Id);
            }
            return totalPath;
        }

        private async Task<List<Node>> GetNeighborsAsync(Guid nodeId)
        {
            return await _nodeService.GetNeighbors(nodeId);
        }

        private async Task<int> GetDistance(Guid currentNodeId, Guid neighborNodeId)
        {
            return await _nodeService.GetDistance(currentNodeId, neighborNodeId);
        }
    }
}
