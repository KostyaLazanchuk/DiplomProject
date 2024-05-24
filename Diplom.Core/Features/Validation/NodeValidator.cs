using Diplom.Core.Const;
using Diplom.Core.Models;
using FluentValidation;

namespace Diplom.Core.Features.Validation
{
    public class NodeValidator : AbstractValidator<Node>
    {
        public NodeValidator() 
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(DataConst.NodeMessageValidation.NameMessage);
            RuleFor(x => x.Id).NotEmpty().WithMessage(DataConst.NodeMessageValidation.IdMessage);
            RuleFor(x => x.Position).NotEmpty().WithMessage(DataConst.NodeMessageValidation.PostionMessage);
        }
    }
}
