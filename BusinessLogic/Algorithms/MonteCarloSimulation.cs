using Diplom.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Algorithms
{
    public class MonteCarloSimulation
    {
        private readonly Random _random = new Random();
        private readonly Dictionary<Guid, double> _failureProbabilities;

        public MonteCarloSimulation(Dictionary<Guid, double> failureProbabilities)
        {
            _failureProbabilities = failureProbabilities;
        }

        public double EvaluateNetworkReliability(List<Node> nodes, int iterations)
        {
            int successfulIterations = 0;

            for (int i = 0; i < iterations; i++)
            {
                if (IsNetworkOperational(nodes))
                {
                    successfulIterations++;
                }
            }

            return (double)successfulIterations / iterations;
        }

        private bool IsNetworkOperational(List<Node> nodes)
        {
            var nodesCopy = CreateNetworkCopyWithFailures(nodes);
            return IsNetworkConnected(nodesCopy);
        }

        private List<Node> CreateNetworkCopyWithFailures(List<Node> nodes)
        {
            var nodesCopy = new List<Node>();

            foreach (var node in nodes)
            {
                var nodeCopy = new Node
                {
                    Id = node.Id,
                    Name = node.Name,
                    CreatedOn = node.CreatedOn,
                    Position = node.Position,
                    Edge = new List<Edge>()
                };

                foreach (var edge in node.Edge)
                {
                    if (_failureProbabilities.TryGetValue(edge.Id, out double failureProbability))
                    {
                        if (_random.NextDouble() > failureProbability)
                        {
                            var edgeCopy = new Edge
                            {
                                Id = edge.Id,
                                Weight = edge.Weight,
                                EndNode = edge.EndNode
                            };

                            nodeCopy.Edge.Add(edgeCopy);
                        }
                    }
                }

                nodesCopy.Add(nodeCopy);
            }

            return nodesCopy;
        }

        private bool IsNetworkConnected(List<Node> nodes)
        {
            if (nodes.Count == 0)
                return false;

            var visited = new HashSet<Guid>();
            var stack = new Stack<Node>();
            stack.Push(nodes[0]);

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                if (!visited.Contains(current.Id))
                {
                    visited.Add(current.Id);
                    foreach (var edge in current.Edge)
                    {
                        var neighbor = nodes.FirstOrDefault(n => n.Id == edge.EndNode);
                        if (neighbor != null && !visited.Contains(neighbor.Id))
                        {
                            stack.Push(neighbor);
                        }
                    }
                }
            }

            return visited.Count == nodes.Count;
        }
    }
}
