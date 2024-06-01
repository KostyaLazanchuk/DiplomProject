using BusinessLogic.Interface;
using DataAccess.Repositories;
using Diplom.Core.Models;

namespace BusinessLogic.Service
{
    public class CommonService : ICommonService
    {
        private readonly CommonRepository _commonRepository;

        public CommonService(CommonRepository commonRepository)
        {
            _commonRepository = commonRepository;
        }

        public Task DeleteAllData() => _commonRepository.DeleteAllData();
        public Task<List<Node>> GetAllNodesWithRelationships() => _commonRepository.GetAllNodesWithRelationships();
        public Task<List<Node>> GetNodesByPattern(string name) => _commonRepository.GetNodesByPattern(name);
        public Task<Node> GetNodeByPatternWithMinIndex(string namePattern) => _commonRepository.GetNodeByPatternWithMinIndex(namePattern);
        public Task CreateRootedNodeCopies(Node baseNode, Node rootNode, int index) => _commonRepository.CreateRootedNodeCopies(baseNode, rootNode, index);
    }
}
