using Diplom.Core.Features.EdgeFeatures.Command;
using Diplom.Core.Features.Validation;
using MediatR;

namespace Diplom.Core.Features.EdgeFeatures.Handler
{
    public class ValidateEdgeHandler : IRequestHandler<VallidateEdgeCommand, Unit>
    {
        private readonly EdgeValidator _edgeValidator;
        public ValidateEdgeHandler(EdgeValidator edgeValidator)
        {
            _edgeValidator = edgeValidator;
        }

        public async Task<Unit> Handle(VallidateEdgeCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _edgeValidator.ValidateAsync(request.Edge);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(error => error.ErrorMessage);
                throw new ValidationException(errors);
            }

            return Unit.Value;
        }
    }
}
