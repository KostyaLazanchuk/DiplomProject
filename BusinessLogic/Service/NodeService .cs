using BusinessLogic.Interface;
using Diplom.Core.Features.Validation;
using DataAccess.Repositories;
using Diplom.Core.Models;

namespace BusinessLogic.Service
{
    public class NodeService : INodeService
    {
        private readonly NodeRepository _nodeRepository;

        public NodeService(NodeRepository nodeRepository)
        {
            _nodeRepository = nodeRepository;
        }

        public Task<Node> GetNodeById(Guid id) => _nodeRepository.GetNodeById(id);
        public Task<Node> GetNodeByName(string name) => _nodeRepository.GetNodeByName(name);
        public Task<Node> CreateNode(Node node) => _nodeRepository.CreateNode(node);
        public Task<Node> UpdateNode(Guid id, string newNodeName) => _nodeRepository.UpdateNode(id, newNodeName);
        public Task<bool> DeleteNode(Guid id) => _nodeRepository.DeleteNode(id);
        public Task<List<Node>> GetNeighbors (Guid id) => _nodeRepository.GetNeighbors(id);
        public Task<int> GetDistance(Guid currentNodeId, Guid neighborNodeId) => _nodeRepository.GetDistance(currentNodeId, neighborNodeId);
        public Task<List<Node>> GetAllNodes() => _nodeRepository.GetAllNodes();
        public Task<int> CountNodes() => _nodeRepository.CountNodes();
        public Task<int> CountNodesByName(string name) => _nodeRepository.CountNodesByName(name);
        public Task<Guid> GetNodeIdByName(string name) => _nodeRepository.GetNodeIdByName(name);
    }
}
