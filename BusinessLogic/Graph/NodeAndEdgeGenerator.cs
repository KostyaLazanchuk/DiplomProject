using BusinessLogic.Interface;
using BusinessLogic.Service;
using Diplom.Core.Models;

namespace BusinessLogic.Graph
{
    public class NodeAndEdgeGenerator
    {
        private readonly INodeService _nodeService;
        private readonly IEdgeService _edgeService;

        public NodeAndEdgeGenerator(INodeService nodeService, IEdgeService edgeService)
        {
            _nodeService = nodeService;
            _edgeService = edgeService;
        }
        public async Task<List<Node>> CreateRandomNodesAndEdges(int nodeCount, string nameNode)
        {
            var random = new Random();
            var nodes = new List<Node>();

            for (int i = 1; i <= nodeCount; i++)
            {
                var nodeId = Guid.NewGuid();
                var node = new Node
                {
                    Id = nodeId,
                    Name = $"{nameNode}{i}",
                    Position = i,
                    CreatedOn = DateTime.UtcNow,
                    Edge = new List<Edge>()
                };

                await CreateNode(node);
                nodes.Add(node);
            }

            for (int i = 0; i < nodeCount; i++)
            {
                int edgeCount = random.Next(1, 4);
                for (int j = 0; j < edgeCount; j++)
                {
                    var sourceNode = nodes[i];
                    var targetNode = nodes[random.Next(nodeCount)];

                    if (sourceNode.Id != targetNode.Id)
                    {
                        var edgeId = Guid.NewGuid();
                        int weight = random.Next(1, 10);

                        await CreateEdgeOneWay(sourceNode.Id, targetNode.Id, weight);

                        sourceNode.Edge.Add(new Edge
                        {
                            Id = edgeId,
                            Weight = weight,
                            EndNode = targetNode.Id
                        });
                    }
                }
            }

            return nodes;
        }

        private async Task CreateNode(Node node)
        {
            await _nodeService.CreateNode(node);
        }

        private async Task CreateEdgeOneWay(Guid sourceNodeId, Guid targerNodeId, int weight)
        {
            await _edgeService.CreateRelationshipOneWay(sourceNodeId, targerNodeId, weight);
        }
    }
}
