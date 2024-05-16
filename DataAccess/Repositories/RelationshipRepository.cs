using DataAccess.Models;
using Neo4j.Driver;

namespace DataAccess.Repositories
{
    public class RelationshipRepository
    {
        private readonly IDriver _driver;

        public RelationshipRepository(IDriver driver)
        {
            _driver = driver;
        }

        public async Task CreateRelationshipOneWay(Guid sourceNodeId, Guid targetNodeId, int weight)
        {
            var session = _driver.AsyncSession();
            try
            {
                await session.ExecuteWriteAsync(async tx =>
                {
                    var query = "MATCH (source:Node { id: $sourceNodeId }), (target:Node { id: $targetNodeId }) " +
                                "CREATE (source)-[rel:CONNECTION { weight: $weight }]->(target)";
                    await tx.RunAsync(query, new { sourceNodeId = sourceNodeId.ToString(), targetNodeId = targetNodeId.ToString(), weight });
                });
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public async Task CreateRelationshipOneToOne(Guid sourceNodeId, Guid targetNodeId)
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

                    foreach (var relationship in node.Relationship)
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

    }
}

