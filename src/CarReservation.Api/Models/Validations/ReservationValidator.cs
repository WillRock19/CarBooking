using CarReservation.Api.Models.Domain;
using FluentValidation;

namespace CarReservation.Api.Models.Validations
{
    public class ReservationValidator : AbstractValidator<Reservation>
    {
        public ReservationValidator()
        {
            RuleFor(x => x.InitialDate)
                .Must(date => date != default)
                .WithMessage("The initial date value must be informed.");

            RuleFor(x => x.DurationInMinutes)
                .GreaterThan(TimeSpan.FromMinutes(0));

            RuleFor(x => x.HasDurationOfTwoHoursTops())
                .Equal(true)
                .WithMessage("The reservation should have a maximum duration of two hours.");

            RuleFor(x => x.StartsInTwentyFourHoursOrLess())
                .Equal(true)
                .WithMessage("The reservation can be taken up to 24 hours ahead.");
        }
    }
}
