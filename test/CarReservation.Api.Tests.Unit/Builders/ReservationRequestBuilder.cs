using CarReservation.Api.Models.DTO.Request;

namespace CarReservation.Api.Tests.Unit.Builders
{
    internal class ReservationRequestBuilder
    {
        private bool emptyReservationDate;
        private bool emptyDurationDate;

        private DateTime? reservationDate;
        private int? durationInMinutes;

        internal ReservationRequestBuilder WithEmptyReservationDate() 
        {
            emptyReservationDate = true;
            return this;
        }

        internal ReservationRequestBuilder WithEmptyDurationInMinutes()
        {
            emptyDurationDate = true;
            return this;
        }

        internal ReservationRequestBuilder WithReservationDate(DateTime reservationDate)
        {
            this.reservationDate = reservationDate;
            return this;
        }

        internal ReservationRequestBuilder WithDurationInMinutes(int durationInMinutes)
        {
            this.durationInMinutes = durationInMinutes;
            return this;
        }


        internal ReservationRequest Build() => new()
        {
            ReservationDate = emptyReservationDate
            ? default
            : reservationDate ?? DateTime.Now.AddMinutes(RandomMinutesUpToTwentyFourHours()),

            DurationInMinutes = emptyDurationDate
            ? default
            : durationInMinutes ?? RandomMinutesUpToTwoHours()
        };

        private int RandomMinutesUpToTwoHours() => new Random().Next(1, 120);

        private int RandomMinutesUpToTwentyFourHours() => new Random().Next(1, 1440);
    }
}
