namespace CarReservation.Api.Models.DTO.Response
{
    /// <summary>
    /// Represents a response for 'create reservation' operation.
    /// </summary>
    public class CreateReservationResponse
    {
        /// <summary>
        /// The reservationId. It might be null, in case the create operation couldn't be fullfilled.
        /// </summary>
        public Guid? ReservationId { get; init; }

        /// <summary>
        /// The carId of the reservation. It might be null, in case the create operation couldn't be fullfilled.
        /// </summary>
        public string? CarId { get; init; }

        /// <summary>
        /// The operation's response message. It might show the reservation's information or the possible errors that might have happened.
        /// </summary>
        public required string Message { get; init; }
    }
}
