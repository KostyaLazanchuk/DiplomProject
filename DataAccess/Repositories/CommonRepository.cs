using Diplom.Core.Models;
using Neo4j.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class CommonRepository
    {
        private readonly IDriver _driver;

        public CommonRepository(IDriver driver)
        {
            _driver = driver;
        }

        public async Task DeleteAllData()
        {
            var session = _driver.AsyncSession();
            try
            {
                await session.ExecuteWriteAsync(async tx =>
                {
                    await tx.RunAsync("MATCH (n) DETACH DELETE n");
                });
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public async Task<List<Node>> GetAllNodesWithRelationships()
        {
            var session = _driver.AsyncSession();
            try
            {
                var result = await session.ExecuteReadAsync(async tx =>
                {
                    var reader = await tx.RunAsync(
                        "MATCH (n:Node) OPTIONAL MATCH (n)-[r]->(m) RETURN n, collect({ id: m.id, weight: r.weight, end: m.id }) as relationships");

                    var nodes = new List<Node>();

                    await reader.ForEachAsync(record =>
                    {
                        var nodeProperties = record["n"].As<INode>().Properties;
                        var relationships = record["relationships"].As<List<Dictionary<string, object>>>();

                        var nodeObject = new Node
                        {
                            Id = Guid.Parse(nodeProperties["id"].As<string>()),
                            Name = nodeProperties["name"].As<string>(),
                            Position = int.Parse(nodeProperties["position"].As<string>()),
                            CreatedOn = DateTime.Parse(nodeProperties["createdOn"].As<string>()),
                            Color = nodeProperties.ContainsKey("color") ? nodeProperties["color"].As<string>() : string.Empty,
                            Edge = new List<Edge>()
                        };

                        if (relationships != null)
                        {
                            foreach (var relationshipData in relationships)
                            {
                                if (relationshipData["id"] != null && relationshipData["weight"] != null && relationshipData["end"] != null)
                                {
                                    var relationship = new Edge
                                    {
                                        Id = Guid.Parse(relationshipData["id"].ToString()),
                                        Weight = Convert.ToInt32(relationshipData["weight"]),
                                        EndNode = Guid.Parse(relationshipData["end"].ToString())
                                    };
                                    nodeObject.Edge.Add(relationship);
                                }
                            }
                        }

                        nodes.Add(nodeObject);
                    });

                    return nodes;
                });

                return result;
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public async Task<List<Node>> GetNodesByPattern(string name)
        {
            var session = _driver.AsyncSession();
            try
            {
                var result = await session.ExecuteReadAsync(async tx =>
                {
                    var reader = await tx.RunAsync(
                        "MATCH (n:Node) WHERE n.name STARTS WITH $name " +
                        "OPTIONAL MATCH (n)-[r]->(m) " +
                        "RETURN n, collect({ id: r.id, weight: r.weight, endNode: m.id }) as edges",
                        new { name });

                    var nodes = new List<Node>();

                    await reader.ForEachAsync(record =>
                    {
                        var nodeProperties = record["n"].As<INode>().Properties;
                        var edgesData = record["edges"].As<List<Dictionary<string, object>>>();

                        var nodeObject = new Node
                        {
                            Id = Guid.Parse(nodeProperties["id"].As<string>()),
                            Name = nodeProperties["name"].As<string>(),
                            Position = int.Parse(nodeProperties["position"].As<string>()),
                            CreatedOn = DateTime.Parse(nodeProperties["createdOn"].As<string>()),
                            Color = nodeProperties.ContainsKey("color") ? nodeProperties["color"].As<string>() : string.Empty,
                            Edge = new List<Edge>()
                        };

                        foreach (var edgeData in edgesData)
                        {
                            if (edgeData["id"] != null && edgeData["weight"] != null && edgeData["endNode"] != null)
                            {
                                var edge = new Edge
                                {
                                    Id = Guid.Parse(edgeData["id"].ToString()),
                                    Weight = Convert.ToInt32(edgeData["weight"]),
                                    EndNode = Guid.Parse(edgeData["endNode"].ToString())
                                };
                                nodeObject.Edge.Add(edge);
                            }
                        }

                        nodes.Add(nodeObject);
                    });

                    return nodes;
                });

                return result;
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public async Task<Node> GetNodeByPatternWithMinIndex(string namePattern)
        {
            var session = _driver.AsyncSession();
            try
            {
                var result = await session.ExecuteReadAsync(async tx =>
                {
                    var reader = await tx.RunAsync(
                        "MATCH (n:Node) WHERE n.name STARTS WITH $namePattern RETURN n",
                        new { namePattern });

                    var nodes = new List<Node>();

                    await reader.ForEachAsync(record =>
                    {
                        var nodeProperties = record["n"].As<INode>().Properties;
                        var nodeObject = new Node
                        {
                            Id = Guid.Parse(nodeProperties["id"].As<string>()),
                            Name = nodeProperties["name"].As<string>(),
                            Position = int.Parse(nodeProperties["position"].As<string>()),
                            CreatedOn = DateTime.Parse(nodeProperties["createdOn"].As<string>()),
                            Color = nodeProperties.ContainsKey("color") ? nodeProperties["color"].As<string>() : string.Empty,
                            Edge = new List<Edge>()
                        };

                        nodes.Add(nodeObject);
                    });

                    return nodes;
                });

                if (result.Any())
                {
                    var nodeWithMinIndex = result
                        .Where(n => int.TryParse(n.Name.Substring(namePattern.Length), out _))
                        .OrderBy(n => int.Parse(n.Name.Substring(namePattern.Length)))
                        .FirstOrDefault();

                    return nodeWithMinIndex;
                }

                return null;
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public async Task CreateRootedNodeCopies(Node baseNode, Node rootNode, int index)
        {
            var session = _driver.AsyncSession();
            try
            {
                await session.ExecuteWriteAsync(async tx =>
                {
                    var newRootNodeId = Guid.NewGuid().ToString();
                    await tx.RunAsync(
                        "CREATE (n:Node { id: $newRootNodeId, name: $name, position: $position, createdOn: $createdOn, color: $color })",
                        new { newRootNodeId, name = $"{rootNode.Name}v{index}", position = rootNode.Position, createdOn = rootNode.CreatedOn.ToString("o"), color = rootNode.Color }
                    );

                    foreach (var edge in rootNode.Edge)
                    {
                        var newEdgeId = Guid.NewGuid().ToString();
                        await tx.RunAsync(
                            "MATCH (source:Node { id: $newRootNodeId }), (target:Node { id: $endNode }) " +
                            "CREATE (source)-[:CONNECTION { id: $newEdgeId, weight: $weight }]->(target)",
                            new { newRootNodeId, endNode = edge.EndNode.ToString(), newEdgeId, weight = edge.Weight }
                        );
                    }

                    var connectionEdgeId = Guid.NewGuid().ToString();
                    await tx.RunAsync(
                        "MATCH (source:Node { id: $baseNodeId }), (target:Node { id: $newRootNodeId }) " +
                        "CREATE (source)-[:CONNECTION { id: $connectionEdgeId, weight: 1 }]->(target)",
                        new { baseNodeId = baseNode.Id.ToString(), newRootNodeId, connectionEdgeId }
                    );
                });
            }
            finally
            {
                await session.CloseAsync();
            }
        }
    }
}
