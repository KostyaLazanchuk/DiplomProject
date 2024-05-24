using Diplom.Core.Models;
using MediatR;

namespace Diplom.Core.Features.EdgeFeatures.Command
{
    public class VallidateEdgeCommand : IRequest<Unit>
    {
        public Edge Edge { get; set; }

        public VallidateEdgeCommand(Edge edge)
        {
            Edge = edge;
        }
    }
    public class ValidationException : Exception
    {
        public ValidationException(IEnumerable<string> errors) : base(string.Join(Environment.NewLine, errors)) { }
    }
}
