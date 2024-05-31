using BusinessLogic.Interface;
using BusinessLogic.Service;
using Diplom.Core.Models;

namespace BusinessLogic.Algorithms
{
    public class CheckAnotherWay
    {
        private readonly INodeService _nodeService;
        private readonly ICommonService _commonService;
        private readonly DijkstraAlgorithm _dijkstraAlgorithm;

        public CheckAnotherWay(INodeService nodeService, ICommonService commonService, DijkstraAlgorithm dijkstraAlgorithm)
        {
            _nodeService = nodeService;
            _commonService = commonService;
            _dijkstraAlgorithm = dijkstraAlgorithm;
        }

        public async Task<List<Node>> CheckAnotherWayAfterDijkstraExecute(Guid startId, Guid goalId)
        {
            var allNodesList = await _commonService.GetAllNodesWithRelationships();
            var dijkstraAlgorithmList = await _dijkstraAlgorithm.FindPathByDijkstra(startId, goalId);
            var elementsToRemove = dijkstraAlgorithmList.Skip(1).Take(dijkstraAlgorithmList.Count - 2).Select(n => n.Id);
            allNodesList.RemoveAll(item => elementsToRemove.Contains(item.Id));
            return await DijkstraAlgorithmLogic(startId, goalId, allNodesList);
        }

        private async Task<List<Node>> DijkstraAlgorithmLogic(Guid startId, Guid goalId, List<Node> nodesList)
        {
            var startNode = await GetNodeById(startId);
            var goalNode = await GetNodeById(goalId);

            var distances = new Dictionary<Guid, int>();
            var previous = new Dictionary<Guid, Guid?>();
            var nodes = new Dictionary<Guid, Node>();

            foreach (var node in nodesList)
            {
                distances[node.Id] = int.MaxValue;
                previous[node.Id] = null;
                nodes[node.Id] = node;
            }

            distances[startNode.Id] = 0;

            var priorityQueue = new List<Node>(nodes.Values.OrderBy(n => distances[n.Id]));

            while (priorityQueue.Count > 0)
            {
                var current = priorityQueue.First();
                priorityQueue.Remove(current);

                if (current.Id == goalNode.Id)
                {
                    var path = await ReconstructPath(previous, goalNode.Id);
                    return path;
                }

                if (current.Edge != null)
                {
                    foreach (var edge in current.Edge)
                    {
                        if (nodes.ContainsKey(edge.EndNode.Value))
                        {
                            var neighbor = nodes[edge.EndNode.Value];
                            var alt = distances[current.Id] + edge.Weight;

                            if (alt < distances[neighbor.Id])
                            {
                                distances[neighbor.Id] = alt;
                                previous[neighbor.Id] = current.Id;
                            }
                        }
                    }

                    priorityQueue = priorityQueue.OrderBy(n => distances[n.Id]).ToList();
                }
            }

            return new List<Node>();
        }


        private async Task<Node> GetNodeById(Guid id)
        {
            return await _nodeService.GetNodeById(id);
        }

        private async Task<List<Node>> ReconstructPath(Dictionary<Guid, Guid?> previous, Guid goalId)
        {
            var path = new List<Node>();
            var currentNodeId = goalId;

            while (currentNodeId != Guid.Empty && previous.ContainsKey(currentNodeId))
            {
                var currentNode = await GetNodeById(currentNodeId);
                path.Add(currentNode);
                if (previous[currentNodeId].HasValue)
                {
                    currentNodeId = previous[currentNodeId].Value;
                }
                else
                {
                    break;
                }
            }

            path.Reverse();
            return path;
        }
    }

}
