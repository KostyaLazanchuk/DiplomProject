using BusinessLogic.Algorithms;
using BusinessLogic.Interface;
using Diplom.Core.Models;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorithmTest
{
    [TestFixture]
    public class DijkstraAlgorithmTests
    {
        private Mock<INodeService> _mockNodeService;
        private Mock<ICommonService> _mockCommonService;
        private DijkstraAlgorithm _dijkstraAlgorithm;

        [SetUp]
        public void SetUp()
        {
            _mockNodeService = new Mock<INodeService>();
            _mockCommonService = new Mock<ICommonService>();
            _dijkstraAlgorithm = new DijkstraAlgorithm(_mockNodeService.Object, _mockCommonService.Object);
        }

        [Test]
        public async Task FindPathByDijkstra_ShouldReturnPath_WhenPathExists()
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

            _mockCommonService.Setup(cs => cs.GetAllNodesWithRelationships()).ReturnsAsync(allNodes);
            _mockNodeService.Setup(ns => ns.GetNodeById(It.IsAny<Guid>())).Returns<Guid>(id => Task.FromResult(allNodes.FirstOrDefault(n => n.Id == id)));

            // Act
            var result = await _dijkstraAlgorithm.FindPathByDijkstra(startNodeId, goalNodeId);

            // Assert
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual(nodeA.Id, result[0].Id);
            Assert.AreEqual(nodeB.Id, result[1].Id);
            Assert.AreEqual(nodeC.Id, result[2].Id);
        }

        [Test]
        public async Task FindPathByDijkstra_ShouldHandleEmptyGraph()
        {
            // Arrange
            var startNodeId = Guid.NewGuid();
            var goalNodeId = Guid.NewGuid();

            _mockCommonService.Setup(cs => cs.GetAllNodesWithRelationships()).ReturnsAsync(new List<Node>());
            _mockNodeService.Setup(ns => ns.GetNodeById(It.IsAny<Guid>())).Returns<Guid>(id => Task.FromResult<Node>(null));

            // Act
            var result = await _dijkstraAlgorithm.FindPathByDijkstra(startNodeId, goalNodeId);

            // Assert
            Assert.IsEmpty(result);
        }

        [Test]
        public async Task FindPathByDijkstra_ShouldReturnDirectPath_WhenStartAndGoalAreSame()
        {
            // Arrange
            var startNodeId = Guid.NewGuid();
            var nodeA = new Node { Id = startNodeId, Name = "A", Edge = new List<Edge>() };

            var allNodes = new List<Node> { nodeA };

            _mockCommonService.Setup(cs => cs.GetAllNodesWithRelationships()).ReturnsAsync(allNodes);
            _mockNodeService.Setup(ns => ns.GetNodeById(It.IsAny<Guid>())).Returns<Guid>(id => Task.FromResult(allNodes.FirstOrDefault(n => n.Id == id)));

            // Act
            var result = await _dijkstraAlgorithm.FindPathByDijkstra(startNodeId, startNodeId);

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(nodeA.Id, result[0].Id);
        }
    }
}
