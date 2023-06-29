using CarReservation.Api.Models.Domain;

namespace CarReservation.Api.Tests.Unit.Builders
{
    internal class ReservationBuilder
    {
        private int reservationId;
        private string? carId;
        private DateTime? initialDate;
        private DateTime? endDate;
        private TimeSpan? durationInMinutes;
        private bool emptyInitialDate;

        internal ReservationBuilder WithId(int reservationId)
        {
            this.reservationId = reservationId;
            return this;
        }

        internal ReservationBuilder WithCarId(string carId)
        {
            this.carId = carId;
            return this;
        }

        internal ReservationBuilder WithInitialDate(DateTime initialDate) 
        {
            this.initialDate = initialDate;
            return this;
        }

        internal ReservationBuilder WithDurationInMinutes(int durationInMinutes)
        {
            this.durationInMinutes = TimeSpan.FromMinutes(durationInMinutes);
            return this;
        }

        internal ReservationBuilder WithEmptyInitialDate()
        {
            emptyInitialDate = true;
            return this;
        }

        internal Reservation Build() 
        {
            var durationOfReservation = durationInMinutes ?? RandomMinutesUpToTwoHours();
            var defaultInitialDate = emptyInitialDate ? default : DateTime.UtcNow;

            return new(
                reservationId,
                carId ?? $"C{new Random().Next()}",
                initialDate ?? defaultInitialDate,
                DurationOfCustomizedInterval() ?? durationOfReservation
            );
        } 

        private TimeSpan RandomMinutesUpToTwoHours() => TimeSpan.FromMinutes(new Random().Next(1, 120));

        private TimeSpan? DurationOfCustomizedInterval() => initialDate != null && endDate != null 
            ? TimeSpan.FromMinutes((endDate.Value - initialDate.Value).TotalMinutes)
            : null;


    }
}
