namespace CarReservation.Api.Models.DTO.Request
{
    /// <summary>
    /// Represents a car creation request.
    /// </summary>
    public class CreateCarRequest
    {
        /// <summary>
        /// The car's make. It should not be null or empty.
        /// </summary>
        public required string Make { get; set; }

        /// <summary>
        /// The car's model. It should not be null or empty.
        /// </summary>
        public required string Model { get; set; }
    }
}
