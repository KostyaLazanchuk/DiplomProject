using Diplom.Core.Models;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Neo4j.Driver;

namespace DataAccess.Repositories
{
    public class NodeRepository
    {
        private readonly IDriver _driver;

        public NodeRepository(IDriver driver)
        {
            _driver = driver;
        }

        public async Task<Node> GetNodeById(Guid id)
        {
            var session = _driver.AsyncSession();
            try
            {
                var result = await session.ExecuteReadAsync(async tx =>
                {
                    var reader = await tx.RunAsync(
                        "MATCH (n:Node) WHERE n.id = $id OPTIONAL MATCH (n)-[r]->(m) RETURN n, collect({ id: m.id, weight: r.weight, end: m.id }) as relationships",
                        new { id = id.ToString() });
                    var record = await reader.SingleAsync();

                    var nodeProperties = record["n"].As<INode>().Properties;
                    var relationships = record["relationships"].As<List<Dictionary<string, object>>>();

                    var nodeObject = new Node
                    {
                        Id = Guid.Parse(nodeProperties["id"].As<string>()),
                        Name = nodeProperties["name"].As<string>(),
                        Position = Convert.ToInt32(nodeProperties["position"]),
                        CreatedOn = DateTime.Parse(nodeProperties["createdOn"].As<string>()),
                        Edge = new List<Edge>()
                    };

                    foreach (var relationshipData in relationships)
                    {
                        if (relationshipData["id"] != null) // Check if relationship data is not null
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

                    return nodeObject;
                });
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
                throw; // Re-throw the exception to be handled by the caller if needed
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public async Task<Node> GetNodeByName(string name)
        {
            var session = _driver.AsyncSession();
            try
            {
                var result = await session.ExecuteReadAsync(async tx =>
                {
                    var reader = await tx.RunAsync(
                        "MATCH (n:Node { name: $name }) OPTIONAL MATCH (n)-[r]->(m) RETURN n, collect({ id: r.id, weight: r.weight, endNode: m.id }) as edges",
                        new { name });

                    var record = await reader.SingleAsync();

                    var nodeProperties = record["n"].As<INode>().Properties;
                    var edgesData = record["edges"].As<List<Dictionary<string, object>>>();

                    var nodeObject = new Node
                    {
                        Id = nodeProperties.ContainsKey("id") ? Guid.Parse(nodeProperties["id"].As<string>()) : Guid.Empty,
                        Name = nodeProperties.ContainsKey("name") ? nodeProperties["name"].As<string>() : string.Empty,
                        Position = nodeProperties.ContainsKey("position") ? int.Parse(nodeProperties["position"].As<string>()) : 0,
                        CreatedOn = nodeProperties.ContainsKey("createdOn") ? DateTime.Parse(nodeProperties["createdOn"].As<string>()) : DateTime.MinValue,
                        Edge = new List<Edge>()
                    };

                    if(edgesData is null)
                    {
                        foreach (var edgeData in edgesData)
                        {
                            var edge = new Edge
                            {
                                Id = edgeData.ContainsKey("id") ? Guid.Parse(edgeData["id"].ToString()) : Guid.Empty,
                                Weight = edgeData.ContainsKey("weight") ? Convert.ToInt32(edgeData["weight"]) : 0,
                                EndNode = edgeData.ContainsKey("endNode") ? Guid.Parse(edgeData["endNode"].ToString()) : (Guid?)null
                            };
                            nodeObject.Edge.Add(edge);
                        }
                    }
                    return nodeObject;
                });

                return result;
            }
            finally
            {
                await session.CloseAsync();
            }

        }


        public async Task<Node> CreateNode(Node node)
        {
            var session = _driver.AsyncSession();
            try
            {
                var result = await session.ExecuteWriteAsync(async tx =>
                {
                    var createdOn = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

                    var reader = await tx.RunAsync(
                        "CREATE (n:Node {id: $id, name: $name, position: $position, createdOn: $createdOn}) RETURN n",
                        new { id = node.Id.ToString(), name = node.Name, position = node.Position, createdOn });
                    var record = await reader.SingleAsync();
                    var createdNode = record["n"].As<INode>();
                    return new Node
                    {
                        Id = Guid.Parse(createdNode.Properties["id"].As<string>()),
                        Name = createdNode.Properties["name"].As<string>(),
                        Position = createdNode.Properties["position"].As<int>(),
                        CreatedOn = DateTime.Now,
                        Edge = new List<Edge>()
                    };
                });
                return result;
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public async Task<Node> UpdateNode(Guid id, string newNodeName)
        {
            var session = _driver.AsyncSession();
            try
            {
                var result = await session.ExecuteWriteAsync(async tx =>
                {
                    var reader = await tx.RunAsync(
                        "MATCH (n:Node {id: $id}) SET n.name = $newName RETURN n",
                        new { id = id.ToString(), newName = newNodeName });
                    var record = await reader.SingleAsync();
                    var updatedNode = record["n"].As<INode>();
                    return new Node
                    {
                        Id = Guid.Parse(updatedNode.Properties["id"].As<string>()),
                        Name = updatedNode.Properties["name"].As<string>(),
                        Position = int.Parse(updatedNode.Properties["position"].As<string>()),
                        CreatedOn = DateTime.Parse(updatedNode.Properties["createdOn"].As<string>()),
                        Edge = new List<Edge>()
                    };
                });
                return result;
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public async Task<bool> DeleteNode(Guid id)
        {
            var session = _driver.AsyncSession();
            try
            {
                var result = await session.ExecuteWriteAsync(async tx =>
                {
                    var reader = await tx.RunAsync(
                        "MATCH (n:Node {id: $id}) DETACH DELETE n RETURN COUNT(n) as deletedCount",
                        new { id = id.ToString() });

                    var record = await reader.SingleAsync();
                    var deletedCount = record["deletedCount"].As<int>();

                    return deletedCount > 0;
                });

                return result;
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public async Task<List<Node>> GetNeighbors(Guid id)
        {
            var nodes = new List<Node>();

            var session = _driver.AsyncSession();
            try
            {
                var result = await session.ExecuteReadAsync(async tx =>
                {
                    var cursor = await tx.RunAsync(
                        "MATCH (n:Node { id: $id })-[r]->(m:Node) RETURN m",
                        new { id = id.ToString() });

                    return await cursor.ToListAsync();
                });

                foreach (var record in result)
                {
                    var mNode = record["m"].As<INode>();
                    var node = new Node
                    {
                        Id = Guid.Parse(mNode.Properties["id"].As<string>()),
                        Name = mNode.Properties["name"].As<string>(),
                        CreatedOn = DateTime.Parse(mNode.Properties["createdOn"].As<string>()),
                        Position = int.Parse(mNode.Properties["position"].As<string>()),
                        Edge = new List<Edge>()
                    };
                    nodes.Add(node);
                }
            }
            finally
            {
                await session.CloseAsync();
            }

            return nodes;
        }

        public async Task<int> GetDistance(Guid currentNodeId, Guid neighborNodeId)
        {
            var session = _driver.AsyncSession();
            try
            {
                var result = await session.ExecuteReadAsync(async tx =>
                {
                    var cursor = await tx.RunAsync(
                        "MATCH (current:Node { id: $currentNodeId })-[r]->(neighbor:Node { id: $neighborNodeId }) RETURN r.weight AS weight",
                        new { currentNodeId = currentNodeId.ToString(), neighborNodeId = neighborNodeId.ToString() });

                    if (await cursor.FetchAsync())
                    {
                        return cursor.Current["weight"].As<int>();
                    }
                    return int.MaxValue;
                });

                return result;
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public async Task<List<Node>> GetAllNodes()
        {
            var session = _driver.AsyncSession();
            try
            {
                var result = await session.ExecuteReadAsync(async tx =>
                {
                    var reader = await tx.RunAsync("MATCH (n:Node) RETURN n");

                    var nodes = new List<Node>();

                    while (await reader.FetchAsync())
                    {
                        var nodeProperties = reader.Current["n"].As<INode>().Properties;

                        nodes.Add(new Node
                        {
                            Id = Guid.Parse(nodeProperties["id"].As<string>()),
                            Name = nodeProperties["name"].As<string>(),
                            Edge = new List<Edge>()
                        });
                    }

                    return nodes;
                });

                return result;
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public async Task<int> CountNodes()
        {
            var session = _driver.AsyncSession();
            try
            {
                var result = await session.ExecuteReadAsync(async tx =>
                {
                    var reader = await tx.RunAsync("MATCH (n:Node) RETURN COUNT(n) as nodeCount");
                    var record = await reader.SingleAsync();
                    return record["nodeCount"].As<int>();
                });
                return result;
            }
            finally
            {
                await session.CloseAsync();
            }
        }


        public async Task<int> CountNodesByName(string name)
        {
            var session = _driver.AsyncSession();
            try
            {
                var result = await session.ExecuteReadAsync(async tx =>
                {
                    var reader = await tx.RunAsync(
                        "MATCH (n:Node) WHERE n.name STARTS WITH $name RETURN count(n) as nodeCount",
                        new { name });

                    var record = await reader.SingleAsync();
                    return record["nodeCount"].As<int>();
                });

                return result;
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public async Task<Guid> GetNodeIdByName(string name)
        {
            var session = _driver.AsyncSession();
            try
            {
                var result = await session.ExecuteReadAsync(async tx =>
                {
                    var cursor = await tx.RunAsync(
                        "MATCH (n:Node { name: $name }) RETURN n.id AS id",
                        new { name });

                    var records = await cursor.ToListAsync();

                    if (records.Any())
                    {
                        return Guid.Parse(records.First()["id"].As<string>());
                    }

                    return Guid.Empty;
                });

                return result;
            }
            finally
            {
                await session.CloseAsync();
            }
        }

    }
}
