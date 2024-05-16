using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interface
{
    public interface IRelationshipService
    {
        Task CreateRelationshipOneWay(Guid sourceNodeId, Guid targetNodeId, int weight);
        Task CreateRelationshipOneToOne(Guid sourceNodeId, Guid targetNodeId);
        Task UpdateNodeWithRelationships(Node node);
    }
}
