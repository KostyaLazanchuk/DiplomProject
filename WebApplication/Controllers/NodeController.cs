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
        private readonly NodeService _nodeService;
        private readonly EdgeService _edgeService;
        private readonly CommonRepository _commonRepository;
        private readonly IMediator _mediator;

        public NodeController(NodeService nodeService, EdgeService edgeService, CommonRepository commonRepository, IMediator mediator)
        {
            _nodeService = nodeService;
            _edgeService = edgeService;
            _commonRepository = commonRepository;
            _mediator = mediator;
        }

        // Додавання нових вузлів зі зв'язками
        [HttpPost("create-nodes-with-edges")]
        public async Task<IActionResult> CreateNodesWithEdges([FromBody] List<Node> nodes)
        {
            await _commonRepository.DeleteAllData();
            if (nodes == null || nodes.Count == 0)
            {
                return BadRequest("Nodes list is null or empty");
            }

            // Спочатку створюємо всі вузли
            var createdNodeList = new List<Node>();
            foreach (var node in nodes)
            {
                node.CreatedOn = DateTime.UtcNow;
                var createdNode = await _nodeService.CreateNode(node);
                createdNodeList.Add(createdNode);
            }

            // Потім створюємо зв'язки для вузлів
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

        // Отримання вузла за ID
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
    }
}
