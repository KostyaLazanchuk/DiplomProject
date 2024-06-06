using BusinessLogic.Algorithms;
using BusinessLogic.Interface;
using BusinessLogic.Service;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DijkstraController : ControllerBase
    {
        private readonly INodeService _nodeService;
        private readonly ICommonService _commonService;
        private readonly IDijkstraAlgorithm _dijkstraAlgorithm;

        public DijkstraController(INodeService nodeService, ICommonService commonService, IDijkstraAlgorithm dijkstraAlgorithm)
        {
            _nodeService = nodeService;
            _commonService = commonService;
            _dijkstraAlgorithm = dijkstraAlgorithm;
        }

        [HttpPost]
        [Route("find-shortest-path")]
        public async Task<IActionResult> FindShortestPath([FromForm] DijkstraRequest request)
        {
            var startNode = await _nodeService.GetNodeByName(request.StartNodeName);
            if (startNode == null)
            {
                return BadRequest($"Start node with name {request.StartNodeName} not found.");
            }

            var goalNode = await _nodeService.GetNodeByName(request.GoalNodeName);
            if (goalNode == null)
            {
                return BadRequest($"Goal node with name {request.GoalNodeName} not found.");
            }

            var checkWay = new CheckAnotherWay(_nodeService, _commonService, _dijkstraAlgorithm);
            var path = await checkWay.CheckAnotherWayAfterDijkstraExecute(startNode.Id, goalNode.Id);

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

        [HttpPost]
        [Route("check-another-way")]
        public async Task<IActionResult> CheckAnotherWay([FromForm] CheckAnotherWayRequest request)
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

            var dijkstraAlgorithm = new DijkstraAlgorithmService(_nodeService, _commonService);
            var checkWay = new CheckAnotherWay(_nodeService, _commonService, dijkstraAlgorithm);
            var path = await checkWay.CheckAnotherWayAfterDijkstraExecute(startNode.Id, goalNode.Id);

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

    public class DijkstraRequest
    {
        public string StartNodeName { get; set; }
        public string GoalNodeName { get; set; }
    }

    public class CheckAnotherWayRequest
    {
        public string StartNodeName { get; set; }
        public string GoalNodeName { get; set; }
    }
}
