namespace CarReservation.Api.Models.DTO.Response
{
    /// <summary>
    /// Represents a response for specific operations related to a reservation.
    /// </summary>
    public class ReservationResponse
    {
        /// <summary>
        /// The Id of the reservation, as stored in the database. It's a GUID, and should never be null.
        /// </summary>
        public required Guid Id { get; init; }

        /// <summary>
        /// The Id of the car associated to the reservation, as stored in the database.  It should never be null.
        /// </summary>
        public required string CarId { get; init; }

        /// <summary>
        /// The initial date of the reservation, as stored in the database. It should never be null.
        /// </summary>
        public required DateTime InitialDate { get; init; }

        /// <summary>
        /// The duration in minutes of the reservation, as stored in the database. It should never be null or zero.
        /// </summary>
        public required int DurationInMinutes { get; init; }
    }
}
