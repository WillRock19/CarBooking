using CarReservation.Api.Models.DTO.Request;
using FluentValidation;

namespace CarReservation.Api.Models.Validations
{
    public class CarRequestValidator : AbstractValidator<CreateCarRequest>
    {
        public CarRequestValidator()
        {
            RuleFor(carRequest => carRequest.Make)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage($"{nameof(CreateCarRequest.Make)} should not be empty.")
                .MinimumLength(2)
                .WithMessage($"{nameof(CreateCarRequest.Make)} should have more than 2 characters.");

            RuleFor(carRequest => carRequest.Model)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage($"{nameof(CreateCarRequest.Model)} should not be empty.")
                .MinimumLength(2)
                .WithMessage($"{nameof(CreateCarRequest.Model)} should have more than 2 characters.");
        }
    }
}
