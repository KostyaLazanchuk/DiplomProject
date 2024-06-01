using Diplom.Core.Models;
using Neo4j.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        public async Task CreateEdgeOneToOne(Guid sourceNodeId, Guid targetNodeId, int weight1, int weight2)
        {
            var session = _driver.AsyncSession();
            try
            {
                await session.ExecuteWriteAsync(async tx =>
                {
                    var edgeId1 = Guid.NewGuid();
                    var edgeId2 = Guid.NewGuid();

                    await tx.RunAsync(
                        "MATCH (source:Node { id: $sourceNodeId }) " +
                        "MATCH (target:Node { id: $targetNodeId }) " +
                        "CREATE (source)-[:CONNECTION { id: $edgeId1, weight: $weight1 }]->(target), " +
                        "(target)-[:CONNECTION { id: $edgeId2, weight: $weight2 }]->(source)",
                        new
                        {
                            sourceNodeId = sourceNodeId.ToString(),
                            targetNodeId = targetNodeId.ToString(),
                            edgeId1 = edgeId1.ToString(),
                            edgeId2 = edgeId2.ToString(),
                            weight1,
                            weight2
                        });
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
                        "SET n.name = $name, n.position = $position, n.createdOn = $createdOn, n.color = $color",
                        new { id = node.Id.ToString(), name = node.Name, position = node.Position, createdOn = node.CreatedOn.ToString("o"), color = node.Color });

                    await tx.RunAsync(
                        "MATCH (n:Node { id: $id })-[r]-() DELETE r",
                        new { id = node.Id.ToString() });

                    foreach (var relationship in node.Edge)
                    {
                        await tx.RunAsync(
                            "MATCH (start:Node { id: $startId }), (end:Node { id: $endId }) " +
                            "CREATE (start)-[:CONNECTION { id: $edgeId, weight: $weight }]->(end)",
                            new { startId = node.Id.ToString(), endId = relationship.EndNode.ToString(), edgeId = relationship.Id.ToString(), weight = relationship.Weight });
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
            var result = await session.RunAsync(
                "MATCH (n:Node { id: $nodeId })-[r:CONNECTION]->(m) RETURN r",
                new { nodeId = nodeId.ToString() });
            var edges = new List<Edge>();

            await foreach (var record in result)
            {
                var relationship = record["r"].As<IRelationship>();
                edges.Add(new Edge
                {
                    Id = Guid.Parse(relationship.Properties["id"].As<string>()),
                    Weight = relationship.Properties["weight"].As<int>(),
                    EndNode = Guid.Parse(relationship.EndNodeId.ToString())
                });
            }

            return edges;
        }

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

        public async Task<int> CountEdges()
        {
            var session = _driver.AsyncSession();
            try
            {
                var result = await session.ExecuteReadAsync(async tx =>
                {
                    var reader = await tx.RunAsync("MATCH ()-[r:CONNECTION]->() RETURN COUNT(r) as edgeCount");
                    var record = await reader.SingleAsync();
                    return record["edgeCount"].As<int>();
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
