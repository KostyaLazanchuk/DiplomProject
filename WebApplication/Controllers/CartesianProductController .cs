using BusinessLogic.Graph;
using BusinessLogic.Interface;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartesianProductController : ControllerBase
    {
        private readonly INodeService _nodeService;
        private readonly IEdgeService _edgeService;
        private readonly ICommonService _commonService;

        public CartesianProductController(INodeService nodeService, IEdgeService edgeService, ICommonService commonService)
        {
            _nodeService = nodeService;
            _edgeService = edgeService;
            _commonService = commonService;
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateCartesianProduct([FromBody] CartesianProductRequest request)
        {
            var cartesianProductGenerator = new CartesianProductOfGraphs(_nodeService, _edgeService);
            var oldGraphList = await _commonService.GetAllNodesWithRelationships();
            var graph1Nodes = await _commonService.GetNodesByPattern(request.Graph1Pattern);
            var graph2Nodes = await _commonService.GetNodesByPattern(request.Graph2Pattern);

            await cartesianProductGenerator.GenerateCartesianProduct(graph1Nodes, graph2Nodes);

            foreach (var node in oldGraphList)
            {
                await _nodeService.DeleteNode(node.Id);
            }

            return Ok("Cartesian product generated successfully.");
        }
    }

    public class CartesianProductRequest
    {
        public string Graph1Pattern { get; set; }
        public string Graph2Pattern { get; set; }
    }
}
