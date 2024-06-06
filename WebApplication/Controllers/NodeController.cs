using BusinessLogic.Graph;
using BusinessLogic.Interface;
using BusinessLogic.Service;
using DataAccess.Repositories;
using Diplom.Core.Features.NodeFeatures.Command;
using Diplom.Core.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApplication.ModelsAPI;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NodeController : ControllerBase
    {
        private readonly INodeService _nodeService;
        private readonly IEdgeService _edgeService;
        private readonly ICommonService _commonService;
        private readonly IMediator _mediator;
        private readonly NodeAndEdgeGenerator _nodeAndEdgeGenerator;

        public NodeController(INodeService nodeService, IEdgeService edgeService, ICommonService commonRepository, IMediator mediator, NodeAndEdgeGenerator nodeAndEdgeGenerator)
        {
            _nodeService = nodeService;
            _edgeService = edgeService;
            _commonService = commonRepository;
            _mediator = mediator;
            _nodeAndEdgeGenerator = nodeAndEdgeGenerator;
        }

        [HttpPost("create-nodes-with-edges")]
        public async Task<IActionResult> CreateNodesWithEdges([FromForm] List<Node> nodes)
        {
            await _commonService.DeleteAllData();
            if (nodes == null || nodes.Count == 0)
            {
                return BadRequest("Nodes list is null or empty");
            }

            var createdNodeList = new List<Node>();
            foreach (var node in nodes)
            {
                node.CreatedOn = DateTime.UtcNow;
                var createdNode = await _nodeService.CreateNode(node);
                createdNodeList.Add(createdNode);
            }

            foreach (var node in nodes)
            {
                if (node.Edge != null)
                {
                    foreach (var edge in node.Edge)
                    {
                        if (edge.EndNode.HasValue)
                        {
                            var endNodeId = edge.EndNode.Value;
                            await _edgeService.CreateRelationshipOneWay(node.Id, endNodeId, edge.Weight);
                        }
                    }
                }
            }

            return Ok("Nodes and edges created successfully");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetNodeById(Guid id)
        {
            var node = await _nodeService.GetNodeById(id);
            if (node == null)
            {
                return NotFound();
            }
            return Ok(node);
        }

        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> AddNode([FromForm] NodeCreateAPI nodeAPI)
        {
            try
            {
                var node = new Node
                {
                    Id = Guid.NewGuid(),
                    Name = nodeAPI.Name,
                    Position = nodeAPI.Position,
                    CreatedOn = DateTime.UtcNow
                };

                var validationCommand = new ValidateNodeCommand(node);
                await _mediator.Send(validationCommand);

                await _nodeService.CreateNode(node);

                return Ok(new { Message = "Node added.", Node = node });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { Errors = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Errors = ex.Message });
            }
        }

        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> DeleteNode([FromForm] DeleteNodeRequest request)
        {
            var node = await _nodeService.GetNodeByName(request.NodeName);
            if (node == null)
            {
                return NotFound($"Node with name {request.NodeName} not found.");
            }

            await _nodeService.DeleteNode(node.Id);

            return Ok("Node deleted successfully.");
        }

        [HttpGet]
        [Route("count")]
        public async Task<IActionResult> CountNodes()
        {
            try
            {
                var nodeCount = await _nodeService.CountNodes();
                return Ok(new { Count = nodeCount });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Errors = ex.Message });
            }
        }

        [HttpPut]
        [Route("update-name")]
        public async Task<IActionResult> UpdateNodeName([FromForm] UpdateNodeRequest request)
        {
            try
            {
                var node = await _nodeService.GetNodeByName(request.nodeNameInput);
                if (node == null)
                {
                    return NotFound(new { Message = $"Node with name {request.nodeNameInput} not found." });
                }

                var updatedNode = await _nodeService.UpdateNode(node.Id, request.newNodeName);

                if (updatedNode != null)
                {
                    return Ok(new { Message = "Node updated.", Node = updatedNode });
                }
                else
                {
                    return BadRequest(new { Message = "Node could not be updated." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Errors = ex.Message });
            }
        }

        [HttpGet]
        [Route("nodes-by-pattern")]
        public async Task<IActionResult> GetListNodeWithPatternName([FromForm] string pattern)
        {
            try
            {
                var nodeList = await _commonService.GetNodesByPattern(pattern);
                return Ok(new { Nodes = nodeList });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Errors = ex.Message });
            }
        }

        [HttpDelete]
        [Route("delete-all")]
        public async Task<IActionResult> DeleteAllData()
        {
            await _commonService.DeleteAllData();
            return Ok("All data deleted successfully.");
        }

        [HttpPost]
        [Route("create-random-nodes-and-edges")]
        public async Task<IActionResult> CreateRandomNodesAndEdges([FromForm] int countNode, [FromForm] string nodeName)
        {
            try
            {
                var nodes = await _nodeAndEdgeGenerator.CreateRandomNodesAndEdges(countNode, nodeName);
                return Ok(new { Message = "Random nodes and edges created successfully.", Nodes = nodes });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Errors = ex.Message });
            }
        }

        public class DeleteNodeRequest
        {
            public string NodeName { get; set; }
        }

        public class UpdateNodeRequest
        {
            public string newNodeName { get; set; }
            public string nodeNameInput { get; set; }
        }
    }
}

