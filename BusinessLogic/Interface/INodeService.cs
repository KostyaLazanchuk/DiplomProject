using DataAccess.Models;

namespace BusinessLogic.Interface
{
    public interface INodeService
    {
        Task<Node> GetNodeById(Guid id);
        Task<Node> GetNodeByName(string name);
        Task<Node> CreateNode(Node node);
        Task<Node> UpdateNode(Guid id, string newNodeName);
        Task<bool> DeleteNode(Guid id);
    }
}
