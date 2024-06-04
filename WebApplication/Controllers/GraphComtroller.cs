using BusinessLogic.Graph;
using DataAccess.Repositories;
using Diplom.Core.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using WebApplication.ModelsAPI;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GraphComtroller
    {
        private readonly CommonRepository _commonRepository;
        private readonly CartesianProduct _cartesianProduct;

        public GraphComtroller(CommonRepository commonRepository, CartesianProduct cartesianProduct)
        {
            _commonRepository = commonRepository;
            _cartesianProduct = cartesianProduct;
        }

        [HttpGet("all-nodes-with-relationships")]
        public async Task<ActionResult<List<Node>>> GetAllNodesWithRelationships()
        {
            var nodes = await _commonRepository.GetAllNodesWithRelationships();
            return nodes;
        }

        //create-random-nodes-and-edges
        //cartesian-product
        /*        [HttpPost]
                [Route("create-random-nodes-and-edges")]
                public async Task<IActionResult> CreateRandomNodesAndEdges([FromForm] int countNode, [FromForm] string nodeName)
                {
                    try
                    {
                        await _nodeAndEdgeGenerator.CreateRandomNodesAndEdges(countNode, nodeName);
                        return Ok(new { Message = "Random nodes and edges created successfully." });
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(500, new { Errors = ex.Message });
                    }
                }*/

        /*        [HttpPost]
                [Route("Cartesian-Product")]
                public async Task<IActionResult> ExecuteCartesianProduct([FromQuery] CartesianProductRequest request)
                {
                    try
                    {
                        await _cartesianProduct.CartesianProductExecution(request.NodeName1, request.NodeName2);
                        //return Ok(new { Nodes = nodeList });
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(500, new { Errors = ex.Message });
                    }
                }*/
    }
}
