using DataAccess.Models;
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
                        "MATCH (n:Node)-[r]->(m) WHERE n.id = $id RETURN n, collect({ id: m.id, weight: r.weight, end: m.id }) as relationships",
                        new { id = id.ToString() });
                    var record = await reader.SingleAsync();

                    var nodeProperties = record["n"].As<INode>().Properties;
                    var relationships = record["relationships"].As<List<Dictionary<string, object>>>();

                    var nodeObject = new Node
                    {
                        Id = Guid.Parse(nodeProperties["id"].As<string>()),
                        Name = nodeProperties["name"].As<string>(),
                        Relationship = new List<Relationship>()
                    };

                    foreach (var relationshipData in relationships)
                    {
                        var relationship = new Relationship
                        {
                            Id = Guid.Parse(relationshipData["id"].ToString()),
                            Weight = Convert.ToInt32(relationshipData["weight"]),
                            EndNode = Guid.Parse(relationshipData["end"].ToString())
                        };
                        nodeObject.Relationship.Add(relationship);
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

        public async Task<Node> GetNodeByName(string name)
        {
            var session = _driver.AsyncSession();
            try
            {
                var result = await session.ExecuteReadAsync(async tx =>
                {
                    var reader = await tx.RunAsync(
                        "MATCH (n:Node { name: $name }) RETURN n",
                        new { name });

                    var record = await reader.SingleAsync();

                    var nodeProperties = record["n"].As<INode>().Properties;

                    var nodeObject = new Node
                    {
                        Id = Guid.Parse(nodeProperties["id"].As<string>()),
                        Name = nodeProperties["name"].As<string>(),
                        Relationship = new List<Relationship>()
                    };

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
                        Relationship = new List<Relationship>()
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
                        Relationship = new List<Relationship>()
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
                await session.ExecuteWriteAsync(async tx =>
                {
                    await tx.RunAsync(
                        "MATCH (n:Node {id: $id}) DELETE n",
                        new { id = id.ToString() });
                });
                return true;
            }
            finally
            {
                await session.CloseAsync();
            }
        }
    }
}
