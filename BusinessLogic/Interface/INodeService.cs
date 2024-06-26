﻿using Diplom.Core.Models;
using Neo4j.Driver;

namespace BusinessLogic.Interface
{
    public interface INodeService
    {
        Task<Node> GetNodeById(Guid id);
        Task<Node> GetNodeByName(string name);
        Task<Node> CreateNode(Node node);
        Task<Node> UpdateNode(Guid id, string newNodeName);
        Task<bool> DeleteNode(Guid id);
        Task<List<Node>> GetNeighbors(Guid id);
        Task<int> GetDistance(Guid currentNodeId, Guid neighborNodeId);
        Task<List<Node>> GetAllNodes();
        Task<int> CountNodes();
        Task<int> CountNodesByName(string name);
        Task<Guid> GetNodeIdByName(string name);
        Task SetNodeColor(Guid nodeId, string color);
        Task<List<Node>> GetNodesByColorAsync(string color);
        Task<Node> GetNodeByPosition(int position);
    }
}
