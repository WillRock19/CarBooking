namespace CarReservation.Api.Models.DTO.Request
{
    public class CreateReservationRequest
    {
        public DateTime ReservationDate { get; set; }
        public int DurationInMinutes { get; set; }
    }
}
