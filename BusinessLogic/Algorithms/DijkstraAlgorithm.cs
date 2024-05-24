using BusinessLogic.Interface;
using Diplom.Core.Models;
using Neo4j.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Algorithms
{
    public class DijkstraAlgorithm
    {
        private readonly INodeService _nodeService;
        private readonly ICommonService _commonService;

        public DijkstraAlgorithm(INodeService nodeService, ICommonService commonService)
        {
            _nodeService = nodeService;
            _commonService = commonService;
        }

        public async Task<List<Node>> FindPathByDijkstra(Guid startId, Guid goalId)
        {
            var startNode = await GetNodeById(startId);
            var goalNode = await GetNodeById(goalId);

            var distances = new Dictionary<Guid, int>();
            var previous = new Dictionary<Guid, Guid?>();
            var nodes = new Dictionary<Guid, Node>();

            var test = await GetAllNodesWithRelationships();
            foreach (var node in await GetAllNodesWithRelationships())
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

                    // Re-sort priority queue
                    priorityQueue = priorityQueue.OrderBy(n => distances[n.Id]).ToList();
                }
            }

            return new List<Node>(); // Return an empty list if there is no path
        }


        private async Task<Node> GetNodeById(Guid id)
        {
            return await _nodeService.GetNodeById(id);
        }

        private async Task<List<Node>> GetAllNodesWithRelationships()
        {
            return await _commonService.GetAllNodesWithRelationships();
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

