using BusinessLogic.Graph;
using BusinessLogic.Interface;
using Diplom.Core.Models;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphTest
{
    [TestFixture]
    public class FatTreeGeneratorTests
    {
        private Mock<INodeService> _mockNodeService;
        private Mock<IEdgeService> _mockEdgeService;
        private FatTreeGenerator _generator;
        private List<Node> _createdNodes;

        [SetUp]
        public void SetUp()
        {
            _mockNodeService = new Mock<INodeService>();
            _mockEdgeService = new Mock<IEdgeService>();
            _generator = new FatTreeGenerator(_mockNodeService.Object, _mockEdgeService.Object);
            _createdNodes = new List<Node>();

            _mockNodeService.Setup(ns => ns.CreateNode(It.IsAny<Node>()))
                .ReturnsAsync((Node node) =>
                {
                    _createdNodes.Add(node);
                    return node;
                });

            _mockEdgeService.Setup(es => es.CreateRelationshipOneWay(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<int>()))
                .Returns(Task.CompletedTask);
        }

        [Test]
        public async Task GenerateFatTree_ShouldCreateCorrectNumberOfNodes()
        {
            // Arrange
            int coreCount = 4;

            // Act
            await _generator.GenerateFatTree(coreCount);

            // Assert
            Assert.AreEqual(coreCount, _createdNodes.Count(n => n.Name.StartsWith("Core")), "Incorrect number of Core nodes.");
            Assert.AreEqual(coreCount * 2, _createdNodes.Count(n => n.Name.StartsWith("Agg")), "Incorrect number of Aggregation nodes.");
            Assert.AreEqual(coreCount * 2, _createdNodes.Count(n => n.Name.StartsWith("Access")), "Incorrect number of Access nodes.");
            Assert.AreEqual(coreCount * 4, _createdNodes.Count(n => n.Name.StartsWith("Server")), "Incorrect number of Server nodes.");
        }
    }
}
