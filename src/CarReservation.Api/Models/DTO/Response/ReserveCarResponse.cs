namespace CarReservation.Api.Models.DTO.Response
{
    public class ReserveCarResponse
    {
        public ReserveCarResponse(int? reservationId, string message)
        {
            ReservationId = reservationId;
            Message = message;
        }

        public int? ReservationId { get; init; }
        public string Message { get; init; }
    }
}
