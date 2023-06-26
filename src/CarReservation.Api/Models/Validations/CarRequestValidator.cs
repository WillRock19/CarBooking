using CarReservation.Api.Models.DTO.Request;
using FluentValidation;

namespace CarReservation.Api.Models.Validations
{
    public class CarRequestValidator : AbstractValidator<CarRequest>
    {
        public CarRequestValidator()
        {
            RuleFor(carRequest => carRequest.Make)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage($"{nameof(CarRequest.Make)} should not be null, empty.")
                .MinimumLength(2)
                .WithMessage($"{nameof(CarRequest.Make)} should have more than 2 characters.");

            RuleFor(carRequest => carRequest.Model)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage($"{nameof(CarRequest.Model)} should not be null, empty.")
                .MinimumLength(2)
                .WithMessage($"{nameof(CarRequest.Model)} should have more than 2 characters.");
        }
    }
}
