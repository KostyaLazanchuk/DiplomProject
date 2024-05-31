using BusinessLogic.Interface;
using BusinessLogic.Service;
using Diplom.Core.Models;

namespace BusinessLogic.Graph
{
    public class RootedProduct
    {
        private readonly INodeService _nodeService;
        private readonly ICommonService _commonService;
        private readonly IEdgeService _edgeService;
        private static readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
        public RootedProduct(INodeService nodeService, ICommonService commonService, IEdgeService edgeService) 
        {
            _nodeService = nodeService;
            _commonService = commonService;
            _edgeService = edgeService;
        }
        public async Task RootedProductExecution(string baseGraphName, string rootGraphName)
        {
            var starterRootGraph = await _commonService.GetNodesByPattern(rootGraphName);
            var baseNodes = await _commonService.GetNodesByPattern(baseGraphName);
            var index = 1;

            foreach (var baseNode in baseNodes)
            {
                var rootNode = await _commonService.GetNodeByPatternWithMinIndex(rootGraphName);

                await _commonService.CreateRootedNodeCopies(baseNode, rootNode, index);
                index++;
            }
            foreach (var starteRoot in starterRootGraph)
            {
                await _nodeService.DeleteNode(starteRoot.Id);
            }
        }
    }
}
