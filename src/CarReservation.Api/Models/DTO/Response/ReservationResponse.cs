namespace CarReservation.Api.Models.DTO.Response
{
    /// <summary>
    /// Represents a response for specific operations related to a reservation.
    /// </summary>
    public class ReservationResponse
    {
        /// <summary>
        /// The reservation's ID. It's a GUID. It should never be null.
        /// </summary>
        public required Guid Id { get; init; }

        /// <summary>
        /// The reservation's car ID.  It should never be null.
        /// </summary>
        public required string CarId { get; init; }

        /// <summary>
        /// The reservation's initial date. It should never be null.
        /// </summary>
        public required DateTime InitialDate { get; init; }

        /// <summary>
        /// The reservation's duation in minutes. It should never be null or zero.
        /// </summary>
        public required int DurationInMinutes { get; init; }
    }
}
