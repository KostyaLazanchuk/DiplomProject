using Diplom.Core.Models;
using MediatR;

namespace Diplom.Core.Features.NodeFeatures.Command
{
    public class ValidateNodeCommand : IRequest<Unit>
    {
        public Node Node {  get; set; }

        public ValidateNodeCommand(Node node)
        {
            Node = node;
        }
    }
    public class ValidationException : Exception
    {
        public ValidationException(IEnumerable<string> errors) : base(string.Join(Environment.NewLine, errors)) { }
    }
}
