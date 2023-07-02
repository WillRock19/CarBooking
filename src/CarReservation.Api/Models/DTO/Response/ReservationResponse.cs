namespace CarReservation.Api.Models.DTO.Response
{
    public class ReservationResponse
    {
        public required Guid Id { get; init; }

        public required string CarId { get; init; }

        public required DateTime InitialDate { get; init; }

        public required int DurationInMinutes { get; init; }
    }
}
