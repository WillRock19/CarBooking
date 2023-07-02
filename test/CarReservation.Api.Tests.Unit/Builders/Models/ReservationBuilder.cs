using CarReservation.Api.Models.Domain;

namespace CarReservation.Api.Tests.Unit.Builders.Models
{
    internal class ReservationBuilder
    {
        private Guid reservationId;
        private string? carId;
        private DateTime? initialDate;
        private TimeSpan? durationInMinutes;
        private bool emptyInitialDate;

        public ReservationBuilder()
        {
            reservationId = Guid.Empty;
        }

        internal ReservationBuilder WithId(Guid reservationId)
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
                durationOfReservation
            );
        }

        private TimeSpan RandomMinutesUpToTwoHours() => TimeSpan.FromMinutes(new Random().Next(1, 120));
    }
}
