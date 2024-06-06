using BusinessLogic.Algorithms;
using BusinessLogic.Service;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class MonteCarloController : ControllerBase
    {
        private readonly CommonService _commonService;

        public MonteCarloController(CommonService commonService)
        {
            _commonService = commonService;
        }

        [HttpPost]
        [Route("execute")]
        public async Task<IActionResult> Simulate([FromForm] MonteCarloRequest request)
        {
            var nodes = await _commonService.GetAllNodesWithRelationships();
            var failureProbabilities = new Dictionary<Guid, double>();

            foreach (var node in nodes)
            {
                foreach (var edge in node.Edge)
                {
                    failureProbabilities[edge.Id] = request.FailureProbability;
                }
            }

            var monteCarloSimulation = new MonteCarloSimulation(failureProbabilities);
            var reliability = monteCarloSimulation.EvaluateNetworkReliability(nodes, request.Iterations);
            return Ok(new { Reliability = reliability });
        }
    }

    public class MonteCarloRequest
    {
        public double FailureProbability { get; set; } = 0.1;
        public int Iterations { get; set; } = 1000;
    }

}
