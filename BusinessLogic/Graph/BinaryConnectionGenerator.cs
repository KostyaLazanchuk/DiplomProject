﻿using BusinessLogic.Interface;
using BusinessLogic.Service;
using Diplom.Core.Models;

namespace BusinessLogic.Graph
{
    public class BinaryConnectionGenerator
    {
        private INodeService _nodeService;
        private IEdgeService _edgeService;

        public BinaryConnectionGenerator(INodeService nodeService, IEdgeService edgeService)
        {
            _nodeService = nodeService;
            _edgeService = edgeService;
        }

        public async Task GenerateBinaryConnections(int serverCount)
        {
            var nodes = new List<Node>();
            var binaryStrings = new List<string>();

            for (var i = 0; i < serverCount; i++)
            {
                var node = new Node
                {
                    Id = Guid.NewGuid(),
                    Name = $"Server{i + 1}",
                    CreatedOn = DateTime.UtcNow,
                    Position = i
                };
                await _nodeService.CreateNode(node);
                nodes.Add(node);

                string binary = Convert.ToString(i, 2).PadLeft((int)Math.Ceiling(Math.Log2(serverCount)), '0');
                binaryStrings.Add(binary);
            }

            for (int i = 0; i < serverCount; i++)
            {
                string binary = binaryStrings[i];

                string shift0 = ShiftBit(binary, '0');
                string shift1 = ShiftBit(binary, '1');

                int newPosition0 = Convert.ToInt32(shift0, 2);
                int newPosition1 = Convert.ToInt32(shift1, 2);


                if (newPosition0 < serverCount && newPosition0 != i)
                {
                    var startNode = nodes.First(n => n.Position == i);
                    var endNode = nodes.First(n => n.Position == newPosition0);

                    if (!await _edgeService.IsEdgeExists(startNode.Id, endNode.Id))
                    {
                        await CreateEdge(startNode.Id, endNode.Id);
                    }
                }

                if (newPosition1 < serverCount && newPosition1 != i)
                {
                    var startNode = nodes.First(n => n.Position == i);
                    var endNode = nodes.First(n => n.Position == newPosition1);

                    if (!await _edgeService.IsEdgeExists(startNode.Id, endNode.Id))
                    {
                        await CreateEdge(startNode.Id, endNode.Id);
                    }
                }
            }
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

        private string ShiftBit(string binary, char newBit)
        {
            return binary.Substring(1) + newBit;
        }
    }
}
