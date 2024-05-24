using DataAccess.Repositories;
using Diplom.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GraphComtroller
    {
        private readonly CommonRepository _commonRepository;

        public GraphComtroller(CommonRepository commonRepository)
        {
            _commonRepository = commonRepository;
        }

        [HttpGet("all-nodes-with-relationships")]
        public async Task<ActionResult<List<Node>>> GetAllNodesWithRelationships()
        {
            var nodes = await _commonRepository.GetAllNodesWithRelationships();
            return nodes;
        }
    }
}
