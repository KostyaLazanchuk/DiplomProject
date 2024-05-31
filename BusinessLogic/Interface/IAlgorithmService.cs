using Diplom.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interface
{
    public interface IAlgorithmService
    {
        Task<List<Node>> FindPathByAStar(Guid startId, Guid goalId, Func<Node, Node, double> heuristic);
        Task<List<Node>> FindPathByDijkstra(Guid startId, Guid goalId);
        Task<List<Node>> CheckAnotherWayAfterDijkstraExecute(Guid startId, Guid goalId);
    }
}
