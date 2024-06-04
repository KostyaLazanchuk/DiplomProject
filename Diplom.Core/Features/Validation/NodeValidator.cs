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
            //RuleFor(node => node.Color).Must(IsColorisChosen).WithMessage(DataConst.NodeMessageValidation.InvalidColor);
        }

/*        private bool IsColorisChosen(string color)
        {
            var validColors = new List<string>
            {
                DataConst.Color.Red,
                DataConst.Color.Orange,
                DataConst.Color.Yellow,
                DataConst.Color.Green,
                DataConst.Color.Blue,
                DataConst.Color.Purple,
                DataConst.Color.White
            };

            return validColors.Contains(color);
        }*/
    }
}
