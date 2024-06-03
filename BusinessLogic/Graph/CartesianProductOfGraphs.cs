using BusinessLogic.Interface;
using Diplom.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Graph
{
    public class CartesianProductOfGraphs
    {
        private readonly INodeService _nodeService;
        private readonly IEdgeService _edgeService;

        public CartesianProductOfGraphs(INodeService nodeService, IEdgeService edgeService)
        {
            _nodeService = nodeService;
            _edgeService = edgeService;
        }

        public async Task GenerateCartesianProduct(List<Node> graph1Nodes, List<Node> graph2Nodes)
        {
            var newNodes = new Dictionary<(Guid, Guid), Node>();

            // Create new nodes for Cartesian product
            foreach (var node1 in graph1Nodes)
            {
                foreach (var node2 in graph2Nodes)
                {
                    var newNode = new Node
                    {
                        Id = Guid.NewGuid(),
                        Name = $"{node1.Name},{node2.Name}",
                        CreatedOn = DateTime.UtcNow,
                        Position = -1,
                        Edge = new List<Edge>()
                    };

                    await _nodeService.CreateNode(newNode);
                    newNodes[(node1.Id, node2.Id)] = newNode;
                }
            }

            // Create edges for Cartesian product
            foreach (var node1 in graph1Nodes)
            {
                foreach (var edge1 in node1.Edge)
                {
                    var targetNode1 = graph1Nodes.First(n => n.Id == edge1.EndNode.Value);

                    foreach (var node2 in graph2Nodes)
                    {
                        var newNode1 = newNodes[(node1.Id, node2.Id)];
                        var newNode2 = newNodes[(targetNode1.Id, node2.Id)];

                        var newEdge = new Edge
                        {
                            Id = Guid.NewGuid(),
                            Weight = edge1.Weight,
                            EndNode = newNode2.Id
                        };

                        await _edgeService.CreateRelationshipOneWay(newNode1.Id, newNode2.Id, newEdge.Weight);
                    }
                }
            }

            foreach (var node2 in graph2Nodes)
            {
                foreach (var edge2 in node2.Edge)
                {
                    var targetNode2 = graph2Nodes.First(n => n.Id == edge2.EndNode.Value);

                    foreach (var node1 in graph1Nodes)
                    {
                        var newNode1 = newNodes[(node1.Id, node2.Id)];
                        var newNode2 = newNodes[(node1.Id, targetNode2.Id)];

                        var newEdge = new Edge
                        {
                            Id = Guid.NewGuid(),
                            Weight = edge2.Weight,
                            EndNode = newNode2.Id
                        };

                        await _edgeService.CreateRelationshipOneWay(newNode1.Id, newNode2.Id, newEdge.Weight);
                    }
                }
            }
        }
    }
}
