using BusinessLogic.Graph;
using BusinessLogic.Interface;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BinaryConnectionController : ControllerBase
    {
        private readonly INodeService _nodeService;
        private readonly IEdgeService _edgeService;

        public BinaryConnectionController(INodeService nodeService, IEdgeService edgeService)
        {
            _nodeService = nodeService;
            _edgeService = edgeService;
        }

        [HttpPost]
        [Route("generate")]
        public async Task<IActionResult> GenerateBinaryConnections([FromBody] BinaryConnectionRequest request)
        {
            if (request.ServerCount <= 0)
            {
                return BadRequest("Invalid number of servers.");
            }

            var generator = new BinaryConnectionGenerator(_nodeService, _edgeService);
            await generator.GenerateBinaryConnections(request.ServerCount);
            return Ok("Binary connections generated successfully.");
        }
    }

    public class BinaryConnectionRequest
    {
        public int ServerCount { get; set; }
    }
}
