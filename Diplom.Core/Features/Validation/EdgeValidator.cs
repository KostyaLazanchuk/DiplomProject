using FluentValidation;
using Diplom.Core.Models;

namespace Diplom.Core.Features.Validation
{
    public class EdgeValidator : AbstractValidator<Edge>
    {
        public EdgeValidator() 
        {
            RuleFor(x => x).NotEmpty();
            RuleFor(x => x.EndNode).NotEmpty();
        }
    }
}
