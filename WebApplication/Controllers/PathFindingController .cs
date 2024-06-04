using BusinessLogic.Algorithms;
using BusinessLogic.Interface;
using Diplom.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PathFindingController : ControllerBase
    {
        private readonly INodeService _nodeService;
        private readonly ICommonService _commonService;

        public PathFindingController(INodeService nodeService, ICommonService commonService)
        {
            _nodeService = nodeService;
            _commonService = commonService;
        }

        [HttpPost]
        [Route("find-shortest-path-astar")]
        public async Task<IActionResult> FindShortestPathAStar([FromBody] AStarRequest request)
        {
            var startNode = await _nodeService.GetNodeByName(request.StartNodeName);
            if (startNode == null)
            {
                return NotFound($"Start node with name {request.StartNodeName} not found.");
            }

            var goalNode = await _nodeService.GetNodeByName(request.GoalNodeName);
            if (goalNode == null)
            {
                return NotFound($"Goal node with name {request.GoalNodeName} not found.");
            }

            Func<Node, Node, double> heuristic = (node1, node2) => 0;

            var aStarAlgorithm = new AStarAlgorithm(_nodeService);
            var path = await aStarAlgorithm.FindPathByAStar(startNode.Id, goalNode.Id, heuristic);

            if (path.Any())
            {
                var result = path.Select(node => node.Name).ToList();
                return Ok(new { Path = result });
            }
            else
            {
                return NotFound("No path found.");
            }
        }
    }

    public class AStarRequest
    {
        public string StartNodeName { get; set; }
        public string GoalNodeName { get; set; }
    }
}
