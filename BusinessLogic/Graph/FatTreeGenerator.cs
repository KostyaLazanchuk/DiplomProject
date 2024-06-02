using BusinessLogic.Service;
using Diplom.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Graph
{
    public class FatTreeGenerator
    {
        private NodeService _nodeService;
        private EdgeService _edgeService;
        private int _index;

        public FatTreeGenerator(NodeService nodeService, EdgeService edgeService)
        {
            _nodeService = nodeService;
            _edgeService = edgeService;
            _index = 1;
        }

        public async Task GenerateFatTree(int coreCount)
        {
            int pods = coreCount;
            var coreNodes = new List<Node>();
            var aggNodes = new List<Node>();
            var accessNodes = new List<Node>();
            var serverNodes = new List<Node>();

            // Create Core nodes
            for (int i = 0; i < coreCount; i++)
            {
                var coreNode = await CreateNode($"Core{i + 1}");
                coreNodes.Add(coreNode);
            }

            // Create Aggregation and Access nodes
            for (int pod = 0; pod < pods; pod++)
            {
                var aggNode1 = await CreateNode($"Agg{_index++}");
                var aggNode2 = await CreateNode($"Agg{_index++}");
                aggNodes.Add(aggNode1);
                aggNodes.Add(aggNode2);

                var accessNode1 = await CreateNode($"Access{_index++}");
                var accessNode2 = await CreateNode($"Access{_index++}");
                accessNodes.Add(accessNode1);
                accessNodes.Add(accessNode2);

                // Connect Aggregation to Core
                for (int i = 0; i < coreCount; i++)
                {
                    await CreateEdge(coreNodes[i].Id, aggNode1.Id);
                    await CreateEdge(coreNodes[i].Id, aggNode2.Id);
                }

                // Connect Access to Aggregation
                await CreateEdge(aggNode1.Id, accessNode1.Id);
                await CreateEdge(aggNode1.Id, accessNode2.Id);
                await CreateEdge(aggNode2.Id, accessNode1.Id);
                await CreateEdge(aggNode2.Id, accessNode2.Id);

                // Create and Connect Servers to Access
                var serverNode1 = await CreateNode($"Server{_index++}");
                var serverNode2 = await CreateNode($"Server{_index++}");
                serverNodes.Add(serverNode1);
                serverNodes.Add(serverNode2);
                await CreateEdge(accessNode1.Id, serverNode1.Id);
                await CreateEdge(accessNode1.Id, serverNode2.Id);

                serverNode1 = await CreateNode($"Server{_index++}");
                serverNode2 = await CreateNode($"Server{_index++}");
                serverNodes.Add(serverNode1);
                serverNodes.Add(serverNode2);
                await CreateEdge(accessNode2.Id, serverNode1.Id);
                await CreateEdge(accessNode2.Id, serverNode2.Id);
            }
        }

        private async Task<Node> CreateNode(string name)
        {
            var node = new Node
            {
                Id = Guid.NewGuid(),
                Name = name,
                CreatedOn = DateTime.UtcNow
            };
            await _nodeService.CreateNode(node);
            return node;
        }

        private async Task CreateEdge(Guid startNodeId, Guid endNodeId)
        {
            var edge = new Edge
            {
                Id = Guid.NewGuid(),
                Weight = 1,
                EndNode = endNodeId
            };
            await _edgeService.CreateRelationshipOneWay(startNodeId, endNodeId, edge.Weight);
        }
    }
}

