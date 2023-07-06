using CarReservation.Api.Interfaces;
using CarReservation.Api.Models.DTO.Request;
using FluentValidation;

namespace CarReservation.Api.Models.Validations
{
    public class ReservationRequestValidator : AbstractValidator<CreateReservationRequest>
    {
        public ReservationRequestValidator(ICurrentDate currentDate)
        {
            RuleFor(reservationRequest => reservationRequest.ReservationDate)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .GreaterThanOrEqualTo(currentDate.DateUtcNow().AddMinutes(5))
                .WithMessage($"'Reservation Date' must be at least five minutes from now.");

            RuleFor(reservationRequest => reservationRequest.DurationInMinutes)
                .Cascade(CascadeMode.Stop)
                .GreaterThan(0);
        }
    }
}
