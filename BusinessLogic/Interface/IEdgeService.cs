﻿using Diplom.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interface
{
    public interface IEdgeService
    {
        Task CreateRelationshipOneWay(Guid sourceNodeId, Guid targetNodeId, int weight);
        Task CreateRelationshipOneToOne(Guid sourceNodeId, Guid targetNodeId, int weight1, int weight2);
        Task UpdateNodeWithRelationships(Node node);
        Task<List<Edge>> GetEdgesByNodeId(Guid nodeId);
        Task<bool> UpdateEdgeWeight(Guid edgeId, int newWeight);
        Task<bool> DeleteEdge(Guid edgeId);
        Task<int> CountEdges();
        Task<bool> IsEdgeExists(Guid startNodeId, Guid endNodeId);
    }
}
