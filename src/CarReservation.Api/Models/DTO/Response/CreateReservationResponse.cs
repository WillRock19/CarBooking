using CarReservation.Api.Models.Domain;

namespace CarReservation.Api.Models.DTO.Response
{
    public class CreateReservationResponse
    {
        public CreateReservationResponse(Guid? reservationId, string message)
        {
            ReservationId = reservationId;
            Message = message;
        }

        public Guid? ReservationId { get; init; }
        public string Message { get; init; }
    }
}
