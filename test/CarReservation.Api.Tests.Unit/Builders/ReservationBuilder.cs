using CarReservation.Api.Models.Domain;

namespace CarReservation.Api.Tests.Unit.Builders
{
    internal class ReservationBuilder
    {
        private int reservationId;
        private string? carId;
        private DateTime? initialDate;
        private DateTime? endDate;
        private int? durationInMinutes;
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

        internal ReservationBuilder WithEndDate(DateTime endDate)
        {
            this.endDate = endDate;
            return this;
        }

        internal ReservationBuilder WithDurationInMinutes(int durationInMinutes)
        {
            this.durationInMinutes = durationInMinutes;
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
                endDate ?? defaultInitialDate.AddMinutes(durationOfReservation),
                DurationOfCustomizedInterval() ?? durationOfReservation
            );
        } 

        private int RandomMinutesUpToTwoHours() => new Random().Next(1, 120);

        private int? DurationOfCustomizedInterval() => initialDate != null && endDate != null 
            ? Convert.ToInt32((endDate.Value - initialDate.Value).TotalMinutes)
            : null;


    }
}
