using BusinessLogic.Graph;
using BusinessLogic.Interface;
using BusinessLogic.Service;
using Diplom.Core.Models;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GraphTest
{
    [TestFixture]
    public class BinaryConnectionGeneratorTests
    {
        private Mock<INodeService> _mockNodeService;
        private Mock<IEdgeService> _mockEdgeService;
        private BinaryConnectionGenerator _generator;

        [SetUp]
        public void SetUp()
        {
            _mockNodeService = new Mock<INodeService>();
            _mockEdgeService = new Mock<IEdgeService>();
            _generator = new BinaryConnectionGenerator(_mockNodeService.Object, _mockEdgeService.Object);
        }

        [Test]
        public async Task GenerateBinaryConnections_ShouldCreateNodes()
        {
            // Arrange
            int serverCount = 8;
            var createdNodes = new List<Node>();

            _mockNodeService.Setup(ns => ns.CreateNode(It.IsAny<Node>()))
                .ReturnsAsync((Node node) => node)
                .Callback<Node>(node => createdNodes.Add(node));

            // Act
            await _generator.GenerateBinaryConnections(serverCount);

            // Assert
            Assert.AreEqual(serverCount, createdNodes.Count);
            for (int i = 0; i < serverCount; i++)
            {
                Assert.IsTrue(createdNodes.Exists(n => n.Position == i));
            }
        }

        [Test]
        public async Task GenerateBinaryConnections_ShouldCreateEdges()
        {
            // Arrange
            int serverCount = 8;
            var createdNodes = new List<Node>();
            var createdEdges = new List<Tuple<Guid, Guid>>();

            _mockNodeService.Setup(ns => ns.CreateNode(It.IsAny<Node>()))
                .ReturnsAsync((Node node) => node)
                .Callback<Node>(node => createdNodes.Add(node));

            _mockEdgeService.Setup(es => es.IsEdgeExists(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(false);

            _mockEdgeService.Setup(es => es.CreateRelationshipOneWay(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<int>()))
                .Returns(Task.CompletedTask)
                .Callback<Guid, Guid, int>((start, end, weight) => createdEdges.Add(Tuple.Create(start, end)));

            // Act
            await _generator.GenerateBinaryConnections(serverCount);

            // Assert
            foreach (var node in createdNodes)
            {
                string binary = Convert.ToString(node.Position, 2).PadLeft(3, '0');
                string shift0 = binary.Substring(1) + '0';
                string shift1 = binary.Substring(1) + '1';

                int newPosition0 = Convert.ToInt32(shift0, 2);
                int newPosition1 = Convert.ToInt32(shift1, 2);

                if (newPosition0 < serverCount && newPosition0 != node.Position)
                {
                    var endNode = createdNodes.Find(n => n.Position == newPosition0);
                    Assert.IsTrue(createdEdges.Exists(e => e.Item1 == node.Id && e.Item2 == endNode.Id));
                }

                if (newPosition1 < serverCount && newPosition1 != node.Position)
                {
                    var endNode = createdNodes.Find(n => n.Position == newPosition1);
                    Assert.IsTrue(createdEdges.Exists(e => e.Item1 == node.Id && e.Item2 == endNode.Id));
                }
            }
        }
    }
}
