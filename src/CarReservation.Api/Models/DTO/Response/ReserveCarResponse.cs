namespace CarReservation.Api.Models.DTO.Response
{
    public class ReserveCarResponse
    {
        public ReserveCarResponse(Guid? reservationId, string message)
        {
            ReservationId = reservationId;
            Message = message;
        }

        public Guid? ReservationId { get; init; }
        public string Message { get; init; }
    }
}
