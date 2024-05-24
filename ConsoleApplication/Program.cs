using BusinessLogic.Service;
using Diplom.Core.Features.Validation;
using DataAccess.Connection;
using Diplom.Core.Const;
using DataAccess.Repositories;
using Diplom.Core.Models;
using FluentValidation;
using System;
using System.Net.WebSockets;
using Diplom.Core.Features.NodeFeatures.Command;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Diplom.Core.Features.NodeFeatures.Handler;
using ValidationException = Diplom.Core.Features.NodeFeatures.Command.ValidationException;
using BusinessLogic.Algorithms;
using BusinessLogic.Interface;
using static System.Net.Mime.MediaTypeNames;

internal class Program
{
    private async static Task Main(string[] args)
    {

        // Створення екземпляру сервісу Neo4j
        await using var neo4jService = new Neo4jService(DataConst.ConnectionData.url, DataConst.ConnectionData.user, DataConst.ConnectionData.password);
        // Ініціалізація сервісів
        var nodeService = new NodeService(new NodeRepository(neo4jService.Driver));
        var relationShipService = new EdgeService(new EdgeRepository(neo4jService.Driver));
        var commonService = new CommonService(new CommonRepository(neo4jService.Driver));


        var serviceProvider = new ServiceCollection()
            .AddTransient<IRequestHandler<ValidateNodeCommand, Unit>, ValidateNodeHandler>()
            .AddTransient<NodeValidator>()
            .AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()))
            .BuildServiceProvider();

        var aStarAlgorithm = new AlgorithmService(new AStarAlgorithm(commonService, nodeService), new DijkstraAlgorithm(nodeService, commonService));
        var mediator = serviceProvider.GetRequiredService<IMediator>();

        while (true)
        {
            Console.WriteLine("1. Add Node");
            Console.WriteLine("2. Add Edge");
            Console.WriteLine("3. Find Shortest Path (Dijkstra)");
            Console.WriteLine("4. Find Shortest Path (A*)");
            Console.WriteLine("5. Delete Node");
            Console.WriteLine("7. Update Node");
            Console.WriteLine("8. Count Node");
            Console.WriteLine("9. Count Edge");
            Console.WriteLine("11. Add Edges one to one");
            //Console.WriteLine("12. StressTest");
            Console.WriteLine("19. Exit");
            Console.WriteLine("20. Delete all");

            Console.Write("Choose option: ");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await AddNode(nodeService);
                    break;

                case "2":
                    await AddEdge(relationShipService, nodeService);
                    break;

                case "3":
                    await FindShortestPathDijkstra(nodeService, commonService);
                    break;

                case "4":
                    await FindShortestPathAStar(nodeService, commonService);
                    break;

                case "5":
                    await DeleteNode(nodeService);
                    break;

                case "7":
                    await UpdateNodeName(nodeService);
                    break;

                case "8":
                    await CountNodes(nodeService);
                    break;

                case "9":
                    await CountEdges(relationShipService);
                    break;

                case "11":
                    await AddEdgeOneToOne(relationShipService, nodeService);
                    break;

/*                case "12":
                    await AddEdgeOneToOne(relationShipService, nodeService);
                    break;*/

                case "19":
                    Environment.Exit(0);
                    break;
                case "20":
                    await DeleteData(commonService);
                    break;

                default:
                    Console.WriteLine("Incorrect choice. Try again.");
                    break;
            }
        }
    }

    private static async Task AddNode(NodeService nodeService)
    {
        Console.Write("Input Node Name: ");
        var nodeName = Console.ReadLine();
        if (!string.IsNullOrEmpty(nodeName))
        {
            Console.Write("Input Node Position: ");
            var nodePosition = int.Parse(Console.ReadLine());

            var node = new Node
            {
                Id = Guid.NewGuid(),
                Name = nodeName,
                Position = nodePosition,
                CreatedOn = DateTime.UtcNow,
                Edge = new List<Edge>()
            };

            await nodeService.CreateNode(node);
            Console.WriteLine("Node added.");
        }
        else 
        {
            Console.WriteLine("IncorrectInput");
        }

    }

    private static async Task AddEdge(EdgeService edgeService, NodeService nodeService)
    {
        Console.Write("Input Source Node Name: ");
        var node1Name = Console.ReadLine();
        var node1 = await nodeService.GetNodeByName(node1Name);
        if (node1 == null)
        {
            Console.WriteLine($"Node with name {node1Name} not found.");
            return;
        }

        Console.Write("Input Target Node Name: ");
        var node2Name = Console.ReadLine();
        var node2 = await nodeService.GetNodeByName(node2Name);
        if (node2 == null)
        {
            Console.WriteLine($"Node with name {node2Name} not found.");
            return;
        }

        Console.Write("Input Edge Weight: ");
        var edgeWeight = int.Parse(Console.ReadLine());

        await edgeService.CreateRelationshipOneWay(node1.Id, node2.Id, edgeWeight);
        Console.WriteLine("Edge added.");
    }

    private static async Task FindShortestPathDijkstra(NodeService nodeService, CommonService commonService)
    {
        Console.Write("Input Start Node Name: ");
        var startNodeName = Console.ReadLine();
        var startNode = await nodeService.GetNodeByName(startNodeName);
        Console.Write("Input Goal Node Name: ");
        var goalNodeName = Console.ReadLine();
        var goalNode = await nodeService.GetNodeByName(goalNodeName);

        var dijkstraAlgorithm = new DijkstraAlgorithm(nodeService, commonService);
        var path = await dijkstraAlgorithm.FindPathByDijkstra(startNode.Id, goalNode.Id);

        if (path.Any())
        {
            Console.WriteLine("Path found:");
            foreach (var node in path)
            {
                Console.Write($"{node.Name} ");
            }
            Console.WriteLine();
        }
        else
        {
            Console.WriteLine("No path found.");
        }
    }

    private static async Task FindShortestPathAStar(NodeService nodeService, CommonService commonService)
    {
        Console.Write("Input Start Node Name: ");
        var startNodeName = Console.ReadLine();
        var startNode = await nodeService.GetNodeByName(startNodeName);
        Console.Write("Input Goal Node Name: ");
        var goalNodeName = Console.ReadLine();
        var goalNode = await nodeService.GetNodeByName(goalNodeName);
        Func<Node, Node, double> heuristic = (node1, node2) => 0; // Replace with actual heuristic function

        //var path = await aStarAlgorithm.FindPathByAStar(test1, test2, heuristic);
        var aStarAlgorithm = new AStarAlgorithm(commonService, nodeService);
        var path = await aStarAlgorithm.FindPathByAStar(startNode.Id, goalNode.Id, heuristic);

        if (path.Any())
        {
            Console.WriteLine("Path found:");
            foreach (var nodeId in path)
            {
                var node = await nodeService.GetNodeById(nodeId.Id);
                Console.Write($"{node.Name} ");
            }
            Console.WriteLine();
        }
        else
        {
            Console.WriteLine("No path found.");
        }
    }

    private static async Task DeleteData(CommonService commonService)
    {
        await commonService.DeleteAllData();
    }

    private static async Task DeleteNode(NodeService nodeService)
    {
        Console.Write("Input Node Name: ");
        var nodeName = Console.ReadLine();
        var node = await nodeService.GetNodeByName(nodeName);
        await nodeService.DeleteNode(node.Id);

        Console.WriteLine("Node delete.");
    }

    private static async Task UpdateNodeName(NodeService nodeService)
    {
        Console.Write("Input Node Name: ");
        var nodeNameInput = Console.ReadLine();
        var node = await nodeService.GetNodeByName(nodeNameInput);
        Console.Write("Input New Node Name: ");
        var newNodeName = Console.ReadLine();

        var updatedNode = await nodeService.UpdateNode(node.Id, newNodeName);

        if (updatedNode != null)
        {
            Console.WriteLine($"Node updated. New Name: {updatedNode.Name}");
        }
        else
        {
            Console.WriteLine("Node not found or could not be updated.");
        }
    }

    private static async Task CountNodes(NodeService nodeService)
    {
        var nodeCount = await nodeService.CountNodes();
        Console.WriteLine($"Total number of nodes: {nodeCount}");
    }

    private static async Task CountEdges(EdgeService edgeService)
    {
        var edgeCount = await edgeService.CountEdges();
        Console.WriteLine($"Total number of edges: {edgeCount}");
    }

    private static async Task AddEdgeOneToOne(EdgeService edgeService, NodeService nodeService)
    {
        Console.Write("Input Source Node Name: ");
        var node1Name = Console.ReadLine();
        var node1 = await nodeService.GetNodeByName(node1Name);
        if (node1 == null)
        {
            Console.WriteLine($"Node with name {node1Name} not found.");
            return;
        }

        Console.Write("Input Target Node Name: ");
        var node2Name = Console.ReadLine();
        var node2 = await nodeService.GetNodeByName(node2Name);
        if (node2 == null)
        {
            Console.WriteLine($"Node with name {node2Name} not found.");
            return;
        }

        Console.Write("Input Edge Weight from Node1 to Node2: ");
        var edgeWeight1 = int.Parse(Console.ReadLine());

        Console.Write("Input Edge Weight from Node2 to Node1: ");
        var edgeWeight2 = int.Parse(Console.ReadLine());

        await edgeService.CreateRelationshipOneToOne(node1.Id, node2.Id, edgeWeight1, edgeWeight2);
        Console.WriteLine("Edge added.");
    }

    private static async Task StressTest(List<Node> nodes, NodeService nodeService, CommonService commonService)
    {
        var failureProbabilities = new Dictionary<Guid, double>
    {
/*        { edgeId1, 0.1 }, // Ймовірність відмови ребра між Node1 та Node2
        { edgeId2, 0.2 }, // Ймовірність відмови ребра між Node1 та Node3
        { edgeId3, 0.05 }, // Ймовірність відмови ребра між Node2 та Node5
        { edgeId4, 0.15 }, // Ймовірність відмови ребра між Node3 та Node4
        { edgeId5, 0.1 } // Ймовірність відмови ребра між Node4 та Node5*/
    };

        var monteCarloSimulation = new MonteCarloSimulation(failureProbabilities);
        var monteCarloService = new SimulationService(monteCarloSimulation);

        Console.WriteLine("Enter number of iterations for Monte Carlo Simulation:");
        int iterations = int.Parse(Console.ReadLine());

        var reliability = monteCarloService.EvaluateNetworkReliability(nodes, iterations);
        Console.WriteLine($"Network reliability: {reliability * 100}%");
    }

    /*    private static async Task DeleteEdgeById(EdgeService edgeService)
        {
            Console.Write("Input Edge Id to Delete: ");
            var edgeIdInput = Console.ReadLine();
            if (Guid.TryParse(edgeIdInput, out var edgeId))
            {
                var success = await edgeService.DeleteEdge(edgeId);

                if (success)
                {
                    Console.WriteLine("Edge deleted.");
                }
                else
                {
                    Console.WriteLine("Edge not found or could not be deleted.");
                }
            }
            else
            {
                Console.WriteLine("Invalid Id format.");
            }
        }*/
}
