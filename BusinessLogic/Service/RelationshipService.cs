using BusinessLogic.Interface;
using DataAccess.Models;
using DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Service
{
    public class RelationshipService : IRelationshipService
    {
        private readonly RelationshipRepository _relationshipRepository;

        public RelationshipService(RelationshipRepository relationshipRepository)
        {
            _relationshipRepository = relationshipRepository;
        }

        public Task CreateRelationshipOneWay(Guid sourceNodeId, Guid targetNodeId, int weight) => _relationshipRepository.CreateRelationshipOneWay(sourceNodeId, targetNodeId, weight);
        public Task CreateRelationshipOneToOne(Guid sourceNodeId, Guid targetNodeId) => _relationshipRepository.CreateRelationshipOneToOne(sourceNodeId, targetNodeId);
        public Task UpdateNodeWithRelationships(Node node) => _relationshipRepository.UpdateNodeWithRelationships(node);
    }
}
