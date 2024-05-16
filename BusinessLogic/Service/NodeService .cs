using BusinessLogic.Interface;
using DataAccess.Models;
using DataAccess.Repositories;

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
    }
}
