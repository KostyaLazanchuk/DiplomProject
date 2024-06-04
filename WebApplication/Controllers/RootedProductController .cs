using BusinessLogic.Graph;
using BusinessLogic.Interface;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RootedProductController : ControllerBase
    {
        private readonly RootedProduct _rootedProduct;

        public RootedProductController(INodeService nodeService, ICommonService commonService, IEdgeService edgeService)
        {
            _rootedProduct = new RootedProduct(nodeService, commonService, edgeService);
        }

        [HttpPost]
        [Route("execute")]
        public async Task<IActionResult> ExecuteRootedProduct([FromBody] RootedProductRequest request)
        {
            await _rootedProduct.RootedProductExecution(request.BaseNodeName, request.RootedNodeName);
            return Ok("Rooted product execution completed successfully.");
        }
    }

    public class RootedProductRequest
    {
        public string BaseNodeName { get; set; }
        public string RootedNodeName { get; set; }
    }
}
