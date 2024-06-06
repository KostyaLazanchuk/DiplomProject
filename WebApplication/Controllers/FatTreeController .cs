using BusinessLogic.Graph;
using BusinessLogic.Interface;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FatTreeController : ControllerBase
    {
        private readonly INodeService _nodeService;
        private readonly IEdgeService _edgeService;

        public FatTreeController(INodeService nodeService, IEdgeService edgeService)
        {
            _nodeService = nodeService;
            _edgeService = edgeService;
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateFatTree([FromForm] FatTreeRequest request)
        {
            if (request.CoreCount <= 0)
            {
                return BadRequest("Invalid number of Core nodes.");
            }

            var fatTreeGenerator = new FatTreeGenerator(_nodeService, _edgeService);
            await fatTreeGenerator.GenerateFatTree(request.CoreCount);
            return Ok("Fat Tree created successfully.");
        }
    }

    public class FatTreeRequest
    {
        public int CoreCount { get; set; }
    }
}
