namespace CarReservation.Api.Models.DTO.Response
{
    public class CreateReservationResponse
    {
        public CreateReservationResponse(Guid? reservationId, string carId, string message)
        {
            ReservationId = reservationId;
            CarId = carId;
            Message = message;
        }

        public Guid? ReservationId { get; init; }
        
        public string? CarId { get; init; }

        public string Message { get; init; }
    }
}
