using CarReservation.Api.Interfaces;

namespace CarReservation.Api.Infraestructure
{
    public class CurrentDate : ICurrentDate
    {
        public DateTime DateUtcNow() => DateTime.UtcNow;
    }
}
