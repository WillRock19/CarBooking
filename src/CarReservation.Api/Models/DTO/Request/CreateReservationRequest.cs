namespace CarReservation.Api.Models.DTO.Request
{
    /// <summary>
    /// Represents a car reservation request.
    /// </summary>
    public class CreateReservationRequest
    {
        /// <summary>
        /// The reservation's date. It should not be empty.
        /// </summary>
        public DateTime ReservationDate { get; set; }

        /// <summary>
        /// The reservation's duration in minutes. It's an integer that should not be empty or zero.
        /// </summary>
        public int DurationInMinutes { get; set; }
    }
}
