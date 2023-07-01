using CarReservation.Api.Models.DTO.Request;

namespace CarReservation.Api.Tests.Unit.Builders.DTO
{
    internal class CreateReservationRequestBuilder
    {
        private bool emptyReservationDate;
        private bool emptyDurationDate;

        private DateTime? reservationDate;
        private int? durationInMinutes;

        internal CreateReservationRequestBuilder WithEmptyReservationDate()
        {
            emptyReservationDate = true;
            return this;
        }

        internal CreateReservationRequestBuilder WithEmptyDurationInMinutes()
        {
            emptyDurationDate = true;
            return this;
        }

        internal CreateReservationRequestBuilder WithReservationDate(DateTime reservationDate)
        {
            this.reservationDate = reservationDate;
            return this;
        }

        internal CreateReservationRequestBuilder WithDurationInMinutes(int durationInMinutes)
        {
            this.durationInMinutes = durationInMinutes;
            return this;
        }


        internal CreateReservationRequest Build() => new()
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
