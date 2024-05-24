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
    }
}
