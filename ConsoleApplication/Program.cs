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
using System.Runtime.CompilerServices;
using BusinessLogic.Graph;
using System.Reflection.Emit;
using Neo4j.Driver;

internal class Program
{
    private async static Task Main(string[] args)
    {

        await using var neo4jService = new Neo4jService(DataConst.ConnectionData.url, DataConst.ConnectionData.user, DataConst.ConnectionData.password);
        var nodeService = new NodeService(new NodeRepository(neo4jService.Driver));
        var relationShipService = new EdgeService(new EdgeRepository(neo4jService.Driver));
        var commonService = new CommonService(new CommonRepository(neo4jService.Driver));


        var serviceProvider = new ServiceCollection()
            .AddTransient<IRequestHandler<ValidateNodeCommand, Unit>, ValidateNodeHandler>()
            .AddTransient<NodeValidator>()
            .AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()))
            .BuildServiceProvider();

        var aStarAlgorithm = new AlgorithmService(new AStarAlgorithm(nodeService), new DijkstraAlgorithm(nodeService, commonService), new CheckAnotherWay(nodeService, commonService, new DijkstraAlgorithmService(nodeService, commonService)));
        var test = new NodeAndEdgeGenerator(nodeService, relationShipService);
        var test2 = new CartesianProduct(commonService, relationShipService);
        var test3 = new RootedProduct(nodeService, commonService, relationShipService);
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
            Console.WriteLine("12. MonteCarlo");
            Console.WriteLine("13. List with patter name");
            Console.WriteLine("14. Find Shortest Path (Dijkstra)");
            Console.WriteLine("15. Rooted Product");
            Console.WriteLine("17. Cartesian product");
            Console.WriteLine("18. Random Generator");
            Console.WriteLine("19. Exit");
            Console.WriteLine("20. Delete all");
            Console.WriteLine("21. FatTree Generator");
            Console.WriteLine("22. Binarty Connectio Generator");
            Console.WriteLine("23. Cartesian Product Generator");
            Console.WriteLine("24. Delete Edge");
            Console.WriteLine("25. Update Edge");

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
                                    break; MonteCarlo*/
                case "12":
                    await MonteCarlo(commonService);
                    break;
                case "13":
                    await GetListNodeWithPatternName(commonService);
                    break;
                case "14":
                    await CheckAnotherWay(nodeService, commonService);
                    break;
                case "15":
                    await RootedProductResult(test3);
                    break;
                case "17":
                    await GetCartesianProductResult(test2);
                    break;
                case "18":
                    await CreateRandomNodesAndEdges(test);
                    break;
                case "19":
                    Environment.Exit(0);
                    break;
                case "20":
                    await DeleteData(commonService);
                    break;
                case "21":
                    await CreateFatTree(nodeService, relationShipService, 4);
                    break;
                case "22":
                    await BinaryConnectionGenerator(nodeService, relationShipService);
                    break;
                case "23":
                    await CreateCartesianProduct(nodeService, relationShipService, commonService);
                    break;
                case "24":
                    await DeleteEdge(nodeService, relationShipService);
                    break;
                case "25":
                    await UpdateEdge(nodeService, relationShipService);
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
        var aStarAlgorithm = new AStarAlgorithm(nodeService);
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

    private static async Task GetCartesianProductResult(CartesianProduct cartesianProduct)
    {
        Console.Write("Input NodeName 1");
        var nodeName1 = Console.ReadLine();
        Console.Write("Input NodeName 2");
        var nodeName2 = Console.ReadLine();
        await cartesianProduct.CartesianProductExecution(nodeName1, nodeName2);
    }

    private static async Task GetListNodeWithPatternName(CommonService commonService)
    {
        Console.Write("Input pattern name");
        var name = Console.ReadLine();
        var nodeList = await commonService.GetNodesByPattern(name);
        var test1 = nodeList;
    }

    private static async Task CreateRandomNodesAndEdges(NodeAndEdgeGenerator nodeAndEdgeGenerator)
    {
        Console.Write("Input count nodes");
        var countNode = int.Parse(Console.ReadLine());
        Console.Write("Input node Name");
        var nodeName = Console.ReadLine();
        await nodeAndEdgeGenerator.CreateRandomNodesAndEdges(countNode, nodeName);
    }

    private static async Task RootedProductResult(RootedProduct rootedProduct)
    {
        Console.WriteLine("Input node base Name");
        var baseNodeName = Console.ReadLine();
        Console.WriteLine("Input node rooted Name");
        var rootedNodeName = Console.ReadLine();
        await rootedProduct.RootedProductExecution(baseNodeName, rootedNodeName);
    }

    private static async Task MonteCarlo(CommonService commonService)
    {
        var nodes = await commonService.GetAllNodesWithRelationships();
        var failureProbabilities = new Dictionary<Guid, double>();
        foreach (var node in nodes)
        {
            foreach (var edge in node.Edge)
            {
                // Приклад: всі ребра мають ймовірність відмови 0.1 (10%)
                failureProbabilities[edge.Id] = 0.1;
            }
        }

        var monteCarloSimulation = new MonteCarloSimulation(failureProbabilities);
        var reliability = monteCarloSimulation.EvaluateNetworkReliability(nodes, 1000);
        Console.WriteLine($"Network reliability: {reliability:P2}");
    }

    private static async Task CheckAnotherWay(NodeService nodeService, CommonService commonService)
    {
        Console.Write("Input Start Node Name: ");
        var startNodeName = Console.ReadLine();
        var startNode = await nodeService.GetNodeByName(startNodeName);
        Console.Write("Input Goal Node Name: ");
        var goalNodeName = Console.ReadLine();
        var goalNode = await nodeService.GetNodeByName(goalNodeName);

        var dijkstraAlgorithm = new DijkstraAlgorithmService(nodeService, commonService);
        var checkWay = new CheckAnotherWay(nodeService, commonService, dijkstraAlgorithm);
        var path = await checkWay.CheckAnotherWayAfterDijkstraExecute(startNode.Id, goalNode.Id);

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

    private static async Task CreateFatTree(NodeService nodeService, EdgeService edgeService, int k)
    {
        var fatTreeGenerator = new FatTreeGenerator(nodeService, edgeService);
        Console.Write("Enter the number of Core nodes: ");
        if (int.TryParse(Console.ReadLine(), out int coreCount))
        {
            await fatTreeGenerator.GenerateFatTree(coreCount);
        }
        else
        {
            Console.WriteLine("Invalid number of Core nodes.");
        }
    }

    private static async Task BinaryConnectionGenerator(NodeService nodeService, EdgeService edgeService)
    {
        var generator = new BinaryConnectionGenerator(nodeService, edgeService);
        int serverCount = 128;
        await generator.GenerateBinaryConnections(serverCount);
    }

    private static async Task CreateCartesianProduct(NodeService nodeService, EdgeService edgeService, CommonService commonService)
    {
        var cartesianProductGenerator = new CartesianProductOfGraphs(nodeService, edgeService);
        var oldGraphList = await commonService.GetAllNodesWithRelationships();
        var graph1Nodes = await commonService.GetNodesByPattern("Server");
        var graph2Nodes = await commonService.GetNodesByPattern("Node");

        await cartesianProductGenerator.GenerateCartesianProduct(graph1Nodes, graph2Nodes);

        foreach ( var node in oldGraphList) 
        {
            await nodeService.DeleteNode(node.Id);
        }
    }

    private static async Task DeleteEdge(NodeService nodeService, EdgeService edgeService)
    {
        var node = await nodeService.GetNodeByName("Server2");

        if (node.Edge != null)
        {
            foreach (var edge in node.Edge)
            {
                await edgeService.DeleteEdge(edge.Id);
            }
        }
    }
    private static async Task UpdateEdge(NodeService nodeService, EdgeService edgeService)
    {
        var node = await nodeService.GetNodeByName("Server1");
        if (node.Edge != null)
        {
            foreach (var edge in node.Edge)
            {
                await edgeService.UpdateEdgeWeight(edge.Id, 10);
            }
        }
    }
}
