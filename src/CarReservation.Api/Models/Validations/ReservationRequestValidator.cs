using CarReservation.Api.Models.DTO.Request;
using FluentValidation;

namespace CarReservation.Api.Models.Validations
{
    public class ReservationRequestValidator : AbstractValidator<ReservationRequest>
    {
        public ReservationRequestValidator()
        {
            RuleFor(reservationRequest => reservationRequest.ReservationDate)
                .Cascade(CascadeMode.Stop)
                .NotEmpty();

            RuleFor(reservationRequest => reservationRequest.DurationInMinutes)
                .Cascade(CascadeMode.Stop)
                .GreaterThan(0);
        }
    }
}
