﻿using BusinessLogic.Algorithms;
using BusinessLogic.Interface;
using DataAccess.Repositories;
using Diplom.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Service
{
    public class AlgorithmService : IAlgorithmService
    {
        private readonly AStarAlgorithm _aStarAlgorithm;
        private readonly DijkstraAlgorithm _dijkstraAlgorithm;

        public AlgorithmService(AStarAlgorithm aStarAlgorithm, DijkstraAlgorithm dijkstraAlgorithm)
        {
            _aStarAlgorithm = aStarAlgorithm;
            _dijkstraAlgorithm = dijkstraAlgorithm;
        }

        public Task<List<Node>> FindPathByAStar(Guid startId, Guid goalId, Func<Node, Node, double> heuristic) => _aStarAlgorithm.FindPathByAStar(startId, goalId, heuristic);
        public Task<List<Node>> FindPathByDijkstra(Guid startId, Guid goalId) => _dijkstraAlgorithm.FindPathByDijkstra(startId, goalId);
    }
}
