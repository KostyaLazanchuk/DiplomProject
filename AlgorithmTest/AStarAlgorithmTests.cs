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
    public class AStarAlgorithmTests
    {
        private Mock<ICommonService> _mockCommonService;
        private Mock<INodeService> _mockNodeService;
        private AStarAlgorithm _aStarAlgorithm;

        [SetUp]
        public void SetUp()
        {
            _mockCommonService = new Mock<ICommonService>();
            _mockNodeService = new Mock<INodeService>();
            _aStarAlgorithm = new AStarAlgorithm(_mockCommonService.Object, _mockNodeService.Object);
        }

        [Test]
        public async Task FindPathByAStar_ShouldReturnPath_WhenPathExists()
        {
            // Arrange
            var startNode = new Node { Id = Guid.NewGuid(), Name = "Start", Edge = new List<Edge>() };
            var goalNode = new Node { Id = Guid.NewGuid(), Name = "Goal", Edge = new List<Edge>() };

            var intermediateNode = new Node { Id = Guid.NewGuid(), Name = "Intermediate", Edge = new List<Edge>() };

            startNode.Edge.Add(new Edge { Id = Guid.NewGuid(), Weight = 1, EndNode = intermediateNode.Id });
            intermediateNode.Edge.Add(new Edge { Id = Guid.NewGuid(), Weight = 1, EndNode = goalNode.Id });

            _mockNodeService.Setup(ns => ns.GetNodeById(startNode.Id)).ReturnsAsync(startNode);
            _mockNodeService.Setup(ns => ns.GetNodeById(goalNode.Id)).ReturnsAsync(goalNode);
            _mockNodeService.Setup(ns => ns.GetNodeById(intermediateNode.Id)).ReturnsAsync(intermediateNode);

            Func<Node, Node, double> heuristic = (node1, node2) => 0;

            // Act
            var path = await _aStarAlgorithm.FindPathByAStar(startNode.Id, goalNode.Id, heuristic);

            // Assert
            Assert.AreEqual(3, path.Count);
            Assert.AreEqual(startNode.Id, path[0].Id);
            Assert.AreEqual(intermediateNode.Id, path[1].Id);
            Assert.AreEqual(goalNode.Id, path[2].Id);
        }

        [Test]
        public async Task FindPathByAStar_ShouldReturnEmpty_WhenNoPathExists()
        {
            // Arrange
            var startNode = new Node { Id = Guid.NewGuid(), Name = "Start", Edge = new List<Edge>() };
            var goalNode = new Node { Id = Guid.NewGuid(), Name = "Goal", Edge = new List<Edge>() };

            _mockNodeService.Setup(ns => ns.GetNodeById(startNode.Id)).ReturnsAsync(startNode);
            _mockNodeService.Setup(ns => ns.GetNodeById(goalNode.Id)).ReturnsAsync(goalNode);

            Func<Node, Node, double> heuristic = (node1, node2) => 0;

            // Act
            var path = await _aStarAlgorithm.FindPathByAStar(startNode.Id, goalNode.Id, heuristic);

            // Assert
            Assert.AreEqual(0, path.Count);
        }

        [Test]
        public async Task FindPathByAStar_ShouldUseHeuristic()
        {
            // Arrange
            var startNode = new Node { Id = Guid.NewGuid(), Name = "Start", Edge = new List<Edge>() };
            var goalNode = new Node { Id = Guid.NewGuid(), Name = "Goal", Edge = new List<Edge>() };

            var intermediateNode = new Node { Id = Guid.NewGuid(), Name = "Intermediate", Edge = new List<Edge>() };

            startNode.Edge.Add(new Edge { Id = Guid.NewGuid(), Weight = 1, EndNode = intermediateNode.Id });
            intermediateNode.Edge.Add(new Edge { Id = Guid.NewGuid(), Weight = 1, EndNode = goalNode.Id });

            _mockNodeService.Setup(ns => ns.GetNodeById(startNode.Id)).ReturnsAsync(startNode);
            _mockNodeService.Setup(ns => ns.GetNodeById(goalNode.Id)).ReturnsAsync(goalNode);
            _mockNodeService.Setup(ns => ns.GetNodeById(intermediateNode.Id)).ReturnsAsync(intermediateNode);

            Func<Node, Node, double> heuristic = (node1, node2) => 1;

            // Act
            var path = await _aStarAlgorithm.FindPathByAStar(startNode.Id, goalNode.Id, heuristic);

            // Assert
            Assert.AreEqual(3, path.Count);
            Assert.AreEqual(startNode.Id, path[0].Id);
            Assert.AreEqual(intermediateNode.Id, path[1].Id);
            Assert.AreEqual(goalNode.Id, path[2].Id);
        }
    }
}
