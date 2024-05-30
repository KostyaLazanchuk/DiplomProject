using BusinessLogic.Service;
using Diplom.Core.Models;

namespace BusinessLogic.Graph
{
    public class CartesianProduct
    {
        private readonly CommonService _commonService;
        private readonly EdgeService _edgeService;
        public CartesianProduct(CommonService commonService, EdgeService edgeService) 
        {
            _commonService = commonService;
            _edgeService = edgeService;
        }

        public async Task CartesianProductExecution(string nodenName1, string nodeName2)
        {
            var getNodesByNamePattern = await GetNodesByNamePattern(nodenName1, nodeName2);
            await GetCartesianProduct(getNodesByNamePattern.serverNodesOne, getNodesByNamePattern.serverNodesTwo);
        }

        private async Task GetCartesianProduct(List<Node> list1, List<Node> list2)
        {
            foreach (var node1 in list1)
            {
                foreach (var node2 in list2)
                {
                    await _edgeService.CreateRelationshipOneToOne(node1.Id, node2.Id, 2, 2);
                }
            }
        }
        private async Task<(List<Node> serverNodesOne, List<Node> serverNodesTwo)> GetNodesByNamePattern(string nodenName1, string nodeName2)
        {
            var allNodes = await _commonService.GetAllNodesWithRelationships();

            var serverNodes = new List<Node>();
            var otherNodes = new List<Node>();

            foreach (var node in allNodes)
            {
                if (node.Name.StartsWith(nodenName1))
                {
                    serverNodes.Add(node);
                }
                else if (node.Name.StartsWith(nodeName2))
                {
                    otherNodes.Add(node);
                }
            }

            return (serverNodes, otherNodes);
        }
    }
}
