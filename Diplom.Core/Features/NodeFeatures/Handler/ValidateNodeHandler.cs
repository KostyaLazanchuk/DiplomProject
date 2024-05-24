using Diplom.Core.Features.NodeFeatures.Command;
using Diplom.Core.Features.Validation;
using MediatR;


namespace Diplom.Core.Features.NodeFeatures.Handler
{
    public class ValidateNodeHandler : IRequestHandler<ValidateNodeCommand, Unit>
    {
        private readonly NodeValidator _nodeValidator;
        public ValidateNodeHandler(NodeValidator nodeValidator)
        {
            _nodeValidator = nodeValidator;
        }

        public async Task<Unit> Handle(ValidateNodeCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _nodeValidator.ValidateAsync(request.Node);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(error => error.ErrorMessage);
                throw new ValidationException(errors);
            }

            return Unit.Value;
        }
    }
}
