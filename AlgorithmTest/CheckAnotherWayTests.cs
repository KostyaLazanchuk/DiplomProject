using BusinessLogic.Algorithms;
using BusinessLogic.Interface;
using BusinessLogic.Service;
using Diplom.Core.Models;
using Moq;
using NUnit.Framework;

namespace AlgorithmTest
{
    [TestFixture]
    public class CheckAnotherWayTests
    {
        private Mock<INodeService> _mockNodeService;
        private Mock<ICommonService> _mockCommonService;
        private DijkstraAlgorithmService _dijkstraAlgorithmService;
        private CheckAnotherWay _checkAnotherWay;

        [SetUp]
        public void SetUp()
        {
            _mockNodeService = new Mock<INodeService>();
            _mockCommonService = new Mock<ICommonService>();
            _dijkstraAlgorithmService = new DijkstraAlgorithmService(_mockNodeService.Object, _mockCommonService.Object);
            _checkAnotherWay = new CheckAnotherWay(_mockNodeService.Object, _mockCommonService.Object, _dijkstraAlgorithmService);
        }

        [Test]
        public async Task CheckAnotherWayAfterDijkstraExecute_ShouldReturnPath_WhenPathExists()
        {
            // Arrange
            var startNodeId = Guid.NewGuid();
            var goalNodeId = Guid.NewGuid();
            var nodeA = new Node { Id = startNodeId, Name = "A", Edge = new List<Edge>() };
            var nodeB = new Node { Id = Guid.NewGuid(), Name = "B", Edge = new List<Edge>() };
            var nodeC = new Node { Id = goalNodeId, Name = "C", Edge = new List<Edge>() };

            nodeA.Edge.Add(new Edge { Id = Guid.NewGuid(), Weight = 1, EndNode = nodeB.Id });
            nodeB.Edge.Add(new Edge { Id = Guid.NewGuid(), Weight = 1, EndNode = nodeC.Id });

            var allNodes = new List<Node> { nodeA, nodeB, nodeC };
            var dijkstraPath = new List<Node> { nodeA, nodeB, nodeC };

            _mockCommonService.Setup(cs => cs.GetAllNodesWithRelationships()).ReturnsAsync(allNodes);
            _mockNodeService.Setup(ns => ns.GetNodeById(It.IsAny<Guid>())).Returns<Guid>(id => Task.FromResult(allNodes.FirstOrDefault(n => n.Id == id)));
            _mockCommonService.Setup(cs => cs.GetAllNodesWithRelationships()).ReturnsAsync(allNodes);
            _mockNodeService.Setup(ns => ns.GetNodeById(It.IsAny<Guid>())).Returns<Guid>(id => Task.FromResult(allNodes.FirstOrDefault(n => n.Id == id)));

            // Act
            var result = await _checkAnotherWay.CheckAnotherWayAfterDijkstraExecute(startNodeId, goalNodeId);

            // Assert
            Assert.AreEqual(1, result.Count);
        }
    }
}
