namespace CarReservation.Api.Models.DTO.Request
{
    public class ReservationRequest
    {
        public DateTime ReservationDate { get; set; }
        public int DurationInMinutes { get; set; }
    }
}
