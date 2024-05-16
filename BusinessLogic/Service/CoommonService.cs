using BusinessLogic.Interface;
using DataAccess.Models;
using DataAccess.Repositories;

namespace BusinessLogic.Service
{
    public class CoommonService : ICommonService
    {
        private readonly CommonRepository _commonRepository;

        public CoommonService(CommonRepository commonRepository)
        {
            _commonRepository = commonRepository;
        }

        public Task DeleteAllData() => _commonRepository.DeleteAllData();
        public Task<List<Node>> GetAllNodesWithRelationships() => _commonRepository.GetAllNodesWithRelationships();
    }
}
