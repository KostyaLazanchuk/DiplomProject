using BusinessLogic.Interface;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EdgeController : ControllerBase
    {
        private readonly IEdgeService _edgeService;
        private readonly INodeService _nodeService;

        public EdgeController(IEdgeService edgeService, INodeService nodeService)
        {
            _edgeService = edgeService;
            _nodeService = nodeService;
        }

        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> AddEdge([FromForm] string sourceNodeName, [FromForm] string targetNodeName, [FromForm] int weight)
        {
            try
            {
                var sourceNode = await _nodeService.GetNodeByName(sourceNodeName);
                if (sourceNode == null)
                {
                    return NotFound(new { Message = $"Node with name {sourceNodeName} not found." });
                }

                var targetNode = await _nodeService.GetNodeByName(targetNodeName);
                if (targetNode == null)
                {
                    return NotFound(new { Message = $"Node with name {targetNodeName} not found." });
                }

                await _edgeService.CreateRelationshipOneWay(sourceNode.Id, targetNode.Id, weight);
                return Ok(new { Message = "Edge added." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Errors = ex.Message });
            }
        }

        [HttpGet]
        [Route("count")]
        public async Task<IActionResult> CountEdges()
        {
            try
            {
                var edgeCount = await _edgeService.CountEdges();
                return Ok(new { Count = edgeCount });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Errors = ex.Message });
            }
        }


        [HttpPost]
        [Route("add-one-to-one")]
        public async Task<IActionResult> AddEdgeOneToOne(string sourceNodeName, string targetNodeName, int weightFromSourceToTarget, int weightFromTargetToSource)
        {
            try
            {
                var sourceNode = await _nodeService.GetNodeByName(sourceNodeName);
                if (sourceNode == null)
                {
                    return NotFound(new { Message = $"Node with name {sourceNodeName} not found." });
                }

                var targetNode = await _nodeService.GetNodeByName(targetNodeName);
                if (targetNode == null)
                {
                    return NotFound(new { Message = $"Node with name {targetNodeName} not found." });
                }

                await _edgeService.CreateRelationshipOneToOne(sourceNode.Id, targetNode.Id, weightFromSourceToTarget, weightFromTargetToSource);
                return Ok(new { Message = "Edge added." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Errors = ex.Message });
            }
        }

        [HttpDelete]
        [Route("delete-by-node-name")]
        public async Task<IActionResult> DeleteEdgesByNodeName([FromBody] DeleteEdgesRequest request)
        {
            var node = await _nodeService.GetNodeByName(request.NodeName);

            if (node == null)
            {
                return NotFound($"Node with name {request.NodeName} not found.");
            }

            if (node.Edge != null)
            {
                foreach (var edge in node.Edge)
                {
                    await _edgeService.DeleteEdge(edge.Id);
                }
            }

            return Ok("Edges deleted successfully.");
        }

        [HttpPut]
        [Route("update-weight-by-node-name")]
        public async Task<IActionResult> UpdateEdgeWeightByNodeName([FromBody] UpdateEdgeWeightRequest request)
        {
            var node = await _nodeService.GetNodeByName(request.NodeName);
            if (node == null)
            {
                return NotFound($"Node with name {request.NodeName} not found.");
            }

            if (node.Edge != null)
            {
                foreach (var edge in node.Edge)
                {
                    await _edgeService.UpdateEdgeWeight(edge.Id, request.NewWeight);
                }
            }

            return Ok("Edge weights updated successfully.");
        }

        public class DeleteEdgesRequest
        {
            public string NodeName { get; set; }
        }

        public class UpdateEdgeWeightRequest
        {
            public string NodeName { get; set; }
            public int NewWeight { get; set; }
        }
    }
}
