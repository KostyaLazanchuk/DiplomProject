using BusinessLogic.Algorithms;
using BusinessLogic.Interface;
using Diplom.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Service
{
    public class SimulationService : ISimulationService
    {
        private readonly MonteCarloSimulation _monteCarloSimulation;
        public SimulationService(MonteCarloSimulation monteCarloSimulation) 
        {
            _monteCarloSimulation = monteCarloSimulation;
        }

        public double EvaluateNetworkReliability(List<Node> nodes, int iterations) => _monteCarloSimulation.EvaluateNetworkReliability(nodes, iterations);
    }
}
