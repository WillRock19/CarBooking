namespace CarReservation.Api.Models.DTO.Response
{
    /// <summary>
    /// Represents a response for specific operations related to a car.
    /// </summary>
    public class CarResponse
    {
        /// <summary>
        /// The Id of the car, as stored in the database. It should never be null.
        /// </summary>
        public required string Id { get; init; }

        /// <summary>
        /// The Make of the car, as stored in the database. It should never be null.
        /// </summary>
        public required string Make { get; init; }

        /// <summary>
        /// The Model of the car, as stored in the database. It should never be null.
        /// </summary>
        public required string Model { get; init; }
    }
}
