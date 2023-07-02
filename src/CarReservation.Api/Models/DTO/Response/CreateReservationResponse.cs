using System.Text.Json.Serialization;

namespace CarReservation.Api.Models.DTO.Response
{
    public class CreateReservationResponse
    {
        [JsonConstructor]
        protected CreateReservationResponse() { }

        public CreateReservationResponse(Guid? reservationId, string carId, string message)
        {
            ReservationId = reservationId;
            CarId = carId;
            Message = message;
        }

        public CreateReservationResponse(string message)
        {
            ReservationId = null;
            CarId = null;
            Message = message;
        }

        public Guid? ReservationId { get; init; }
        
        public string? CarId { get; init; }

        public string Message { get; init; }
    }
}
