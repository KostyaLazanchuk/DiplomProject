using BusinessLogic.Service;
using DataAccess.Connection;
using DataAccess.Const;
using DataAccess.Models;
using DataAccess.Repositories;
using System;
using System.Net.WebSockets;

internal class Program
{
    private async static Task Main(string[] args)
    {
        // Створення екземпляру сервісу Neo4j
        await using var neo4jService = new Neo4jService(DataConst.ConnectionData.url, DataConst.ConnectionData.user, DataConst.ConnectionData.password);

        // Ініціалізація сервісів
        var nodeService = new NodeService(new NodeRepository(neo4jService.Driver));
        var relationShipService = new RelationshipService(new RelationshipRepository(neo4jService.Driver));
        var commonService = new CoommonService(new CommonRepository(neo4jService.Driver));

        // Створення нового вузла
/*        var newNode = new Node
        {
            Id = Guid.NewGuid(),
            Name = "New Node"
        };*/
        /*        var createdNode = await nodeService.CreateNode(newNode);
                Console.WriteLine($"Created Node: {createdNode.Name}");*/

        // Отримання вузла
        /*        var fetchedNode = await nodeService.GetNodeById(createdNode.Id);
                Console.WriteLine($"Fetched Node: {fetchedNode.Name}");*/

        // Оновлення вузла
        /*        var updatedNode = await nodeService.UpdateNode(fetchedNode.Id, "Updated Node Name");
                Console.WriteLine($"Updated Node: {updatedNode.Name}");*/

        /*        var findNode = await nodeService.GetNodeById(new Guid("02b98211-c97f-4494-a66d-1f3a59bde75a"));
                var test1 = findNode;*/

        // Видалення вузла
        /*        var isDeleted = await nodeService.DeleteNode(updatedNode.Id);
                Console.WriteLine($"Node Deleted: {isDeleted}");*/

        //await commonService.DeleteAllData();
        /*        var sourceNodeId = new Guid("141c939b-fb49-4951-8cff-8524b1c72231");
                var targetNodeId = new Guid("02b98211-c97f-4494-a66d-1f3a59bde75a");
                await nodeService.CreateFriendshipOneWay(sourceNodeId, targetNodeId);*/
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        var id3 = Guid.NewGuid();
        var id4 = Guid.NewGuid();
        var id5 = Guid.NewGuid();

        var node = new Node
        {
            Id = id1,
            Name = "Server 1",
            Position = 1,
            Relationship = new List<Relationship>
            {
                new Relationship
                {
                    Id = Guid.NewGuid(),
                    Weight = 2,
                    EndNode = id2
                },
                new Relationship
                {
                    Id = Guid.NewGuid(),
                    Weight = 3,
                    EndNode = id3
                }
                // Добавьте другие отношения вашего узла здесь...
            }
        };

        var node2 = new Node
        {
            Id = id2,
            Name = "Server 2",
            Position = 2,
            Relationship = new List<Relationship>
            {
                new Relationship
                {
                    Id = Guid.NewGuid(),
                    Weight = 5,
                    EndNode = id4
                }
                // Добавьте другие отношения вашего узла здесь...
            }
        };

        var node3 = new Node
        {
            Id = id3,
            Name = "Server 3",
            Position = 2,
            Relationship = new List<Relationship>
            {
                new Relationship
                {
                    Id = Guid.NewGuid(),
                    Weight = 1,
                    EndNode = id5
                }
                // Добавьте другие отношения вашего узла здесь...
            }
        };

        var node4 = new Node
        {
            Id = id4,
            Name = "Server 4",
            Position = 3,
            Relationship = new List<Relationship>
            {
                new Relationship
                {
                    Id = Guid.NewGuid(),
                    Weight = 2,
                    EndNode = id5
                }
                // Добавьте другие отношения вашего узла здесь...
            }
        };

        var node5 = new Node
        {
            Id = id5,
            Name = "Server 5",
            Position = 4,
            Relationship = new List<Relationship>
            {
            }
        };

        /*      var createdNode = await nodeService.CreateNode(node);
                var createdNode2 = await nodeService.CreateNode(node2);
                var createdNode3 = await nodeService.CreateNode(node3);
                var createdNode4 = await nodeService.CreateNode(node4);
                var createdNode5 = await nodeService.CreateNode(node5);*/

        await relationShipService.CreateRelationshipOneWay(new Guid("034174d7-13c8-41b6-b932-5ae4c3d37bc5"), new Guid("194d74e3-c61c-4922-bfa1-39399b114ba8"), 6);

        //Console.WriteLine($"Created Node: {createdNode.Name}");
        //await nodeService.UpdateNodeWithRelationships(node);

        //	8cd2c153-7948-4bb9-b804-e43d7a05d6b2
        //	141c939b-fb49-4951-8cff-8524b1c72231

        //await nodeService.CreateFriendshipOneToOne(new Guid("f45de6d6-b34a-4e2b-8fa6-9bc5c62ebb5f"), new Guid ("37b2e7a1-5e03-47f9-b99b-8040b383effc"));
        /*        var test1 = await nodeService.GetNodeById(new Guid("8cd2c153-7948-4bb9-b804-e43d7a05d6b2"));
                var test2 = test1;
                Console.WriteLine("Success");*/

        /*        var test1 = await commonService.GetAllNodesWithRelationships();
                var test2 = test1;*/
    }
}