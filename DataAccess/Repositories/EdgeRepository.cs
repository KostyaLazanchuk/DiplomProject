using Diplom.Core.Models;
using Neo4j.Driver;

namespace DataAccess.Repositories
{
    public class EdgeRepository
    {
        private readonly IDriver _driver;

        public EdgeRepository(IDriver driver)
        {
            _driver = driver;
        }

        public async Task CreateEdgeOneWay(Guid sourceNodeId, Guid targetNodeId, int weight)
        {
            var session = _driver.AsyncSession();
            try
            {
                await session.ExecuteWriteAsync(async tx =>
                {
                    var edgeId = Guid.NewGuid();
                    var query = "MATCH (source:Node { id: $sourceNodeId }), (target:Node { id: $targetNodeId }) " +
                                "CREATE (source)-[rel:CONNECTION { id: $edgeId, weight: $weight }]->(target)";
                    await tx.RunAsync(query, new { sourceNodeId = sourceNodeId.ToString(), targetNodeId = targetNodeId.ToString(), edgeId = edgeId.ToString(), weight });
                });
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public async Task CreateEdgeOneToOne(Guid sourceNodeId, Guid targetNodeId)
        {
            var session = _driver.AsyncSession();
            try
            {
                await session.ExecuteWriteAsync(async tx =>
                {
                    await tx.RunAsync(
                        "MATCH (source:Node { id: $sourceNodeId }) " +
                        "MATCH (target:Node { id: $targetNodeId }) " +
                        "CREATE (source)-[:CONNECTION]->(target), (target)-[:CONNECTION]->(source)",
                        new { sourceNodeId = sourceNodeId.ToString(), targetNodeId = targetNodeId.ToString() });
                });
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public async Task UpdateNodeWithRelationships(Node node)
        {
            var session = _driver.AsyncSession();
            try
            {
                await session.ExecuteWriteAsync(async tx =>
                {
                    await tx.RunAsync(
                        "MATCH (n:Node { id: $id }) " +
                        "SET n.Name = $name, n.SomeOtherProperty = $someOtherValue",
                        new { id = node.Id.ToString(), name = node.Name, someOtherValue = node.Id.ToString() });

                    await tx.RunAsync(
                        "MATCH (n:Node { id: $id })-[r]-() DELETE r",
                        new { id = node.Id.ToString() });

                    foreach (var relationship in node.Edge)
                    {
                        await tx.RunAsync(
                            "MATCH (start:Node { id: $startId }), (end:Node { id: $endId }) " +
                            "CREATE (start)-[:Weight $weight { weight: $weight }]->(end)",
                            new { startId = node.Id.ToString(), endId = relationship.EndNode.ToString(), weight = relationship.Weight });
                    }
                });
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public async Task<List<Edge>> GetEdgesByNodeId(Guid nodeId)
        {
            var session = _driver.AsyncSession();
            var result = await session.RunAsync("MATCH (n:Node {Id: $nodeId})-[:EDGE]->(m) RETURN n, m", new { nodeId });
            var edges = new List<Edge>();

            await foreach (var record in result)
            {
                var edgeNode = record["m"].As<INode>();
                edges.Add(new Edge
                {
                    Id = Guid.NewGuid(),
                    Weight = edgeNode.Properties["Weight"].As<int>(),
                    EndNode = Guid.Parse(edgeNode.Properties["Id"].As<string>())
                });
            }

            return edges;
        }

        //TRASH
        public async Task<bool> UpdateEdgeWeight(Guid edgeId, int newWeight)
        {
            var session = _driver.AsyncSession();
            try
            {
                var result = await session.ExecuteWriteAsync(async tx =>
                {
                    var reader = await tx.RunAsync(
                        "MATCH ()-[r { id: $edgeId }]->() SET r.weight = $newWeight RETURN COUNT(r) as updatedCount",
                        new { edgeId = edgeId.ToString(), newWeight });

                    var record = await reader.SingleAsync();
                    var updatedCount = record["updatedCount"].As<int>();

                    return updatedCount > 0;
                });

                return result;
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public async Task<bool> DeleteEdge(Guid edgeId)
        {
            var session = _driver.AsyncSession();
            try
            {
                var result = await session.ExecuteWriteAsync(async tx =>
                {
                    var reader = await tx.RunAsync(
                        "MATCH ()-[r {id: $edgeId}]->() DELETE r RETURN COUNT(r) as deletedCount",
                        new { edgeId = edgeId.ToString() });

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

    }
}

