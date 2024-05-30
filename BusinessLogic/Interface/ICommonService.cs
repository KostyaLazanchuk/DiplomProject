using Diplom.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interface
{
    public interface ICommonService
    {
        Task DeleteAllData();
        Task<List<Node>> GetAllNodesWithRelationships();
        Task<List<Node>> GetNodesByPattern(string name);
        Task<Node> GetNodeByPatternWithMinIndex(string namePattern);
        Task CreateRootedNodeCopies(Node baseNode, Node rootNode, int index);

    }
}
