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
    public class EdgeService : IEdgeService
    {
        private readonly EdgeRepository _relationshipRepository;

        public EdgeService(EdgeRepository relationshipRepository)
        {
            _relationshipRepository = relationshipRepository;
        }

        public Task CreateRelationshipOneWay(Guid sourceNodeId, Guid targetNodeId, int weight) => _relationshipRepository.CreateEdgeOneWay(sourceNodeId, targetNodeId, weight);
        public Task CreateRelationshipOneToOne(Guid sourceNodeId, Guid targetNodeId) => _relationshipRepository.CreateEdgeOneToOne(sourceNodeId, targetNodeId);
        public Task UpdateNodeWithRelationships(Node node) => _relationshipRepository.UpdateNodeWithRelationships(node);
        public Task<List<Edge>> GetEdgesByNodeId(Guid nodeId) => _relationshipRepository.GetEdgesByNodeId(nodeId);
        public Task<bool> UpdateEdgeWeight(Guid edgeId, int newWeight) => _relationshipRepository.UpdateEdgeWeight(edgeId, newWeight);
        public Task<bool> DeleteEdge(Guid edgeId) => _relationshipRepository.DeleteEdge(edgeId);
    }
}
