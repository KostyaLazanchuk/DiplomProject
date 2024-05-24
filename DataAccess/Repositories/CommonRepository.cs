using Diplom.Core.Models;
using Neo4j.Driver;

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
    }
}
