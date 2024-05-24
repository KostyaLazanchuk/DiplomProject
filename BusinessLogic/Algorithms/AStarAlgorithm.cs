using BusinessLogic.Interface;
using DataAccess.Repositories;
using Diplom.Core.Models;
using Neo4j.Driver;

namespace BusinessLogic.Algorithms
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

        public async Task<List<Node>> FindPathByAStar(Guid startId, Guid goalId, Func<Node, Node, double> heuristic)
        {
            var openSet = new HashSet<Node>();
            var cameFrom = new Dictionary<Node, Node>();

            var gScore = new Dictionary<Node, double>();
            var fScore = new Dictionary<Node, double>();

            var start = await _nodeService.GetNodeById(startId);
            var goal = await _nodeService.GetNodeById(goalId);

            openSet.Add(start);
            gScore[start] = 0;
            fScore[start] = heuristic(start, goal);

            while (openSet.Count > 0)
            {
                var current = GetNodeWithLowestFScore(openSet, fScore);

                if (current.Id == goal.Id)
                {
                    return ReconstructPath(cameFrom, current);
                }

                openSet.Remove(current);

                foreach (var edge in current.Edge)
                {
                    var targetNode = await _nodeService.GetNodeById(edge.EndNode.Value);
                    var tentativeGScore = gScore[current] + edge.Weight;

                    if (!gScore.ContainsKey(targetNode) || tentativeGScore < gScore[targetNode])
                    {
                        cameFrom[targetNode] = current;
                        gScore[targetNode] = tentativeGScore;
                        fScore[targetNode] = gScore[targetNode] + heuristic(targetNode, goal);

                        if (!openSet.Contains(targetNode))
                        {
                            openSet.Add(targetNode);
                        }
                    }
                }
            }

            return new List<Node>(); // Return an empty list if there is no path
        }

        private static Node GetNodeWithLowestFScore(HashSet<Node> openSet, Dictionary<Node, double> fScore)
        {
            Node lowest = null;
            double lowestScore = double.PositiveInfinity;

            foreach (var node in openSet)
            {
                if (fScore.TryGetValue(node, out double score) && score < lowestScore)
                {
                    lowestScore = score;
                    lowest = node;
                }
            }

            return lowest;
        }

        private static List<Node> ReconstructPath(Dictionary<Node, Node> cameFrom, Node current)
        {
            var totalPath = new List<Node> { current };

            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                totalPath.Add(current);
            }

            totalPath.Reverse();
            return totalPath;
        }


        private async Task<List<Node>> GetNodes()
        {
            return await _commonService.GetAllNodesWithRelationships();
        }

        private int HeuristicCostEstimate(Node start, Node goal)
        {
            return Math.Abs(start.Position - goal.Position);
        }

        private async Task<List<Node>> GetNeighbors(Guid nodeId)
        {
            return await _nodeService.GetNeighbors(nodeId);
        }

        private async Task<int> GetDistance(Guid currentNodeId, Guid neighborNodeId)
        {
            return await _nodeService.GetDistance(currentNodeId, neighborNodeId);
        }
    }
}
