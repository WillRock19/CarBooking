using CarReservation.Api.Interfaces;

namespace CarReservation.Api.Models.Domain
{
    public record Reservation(Guid Id, string CarId, DateTime InitialDate, TimeSpan DurationInMinutes) 
    {
        public DateTime EndDate { get; } = InitialDate + DurationInMinutes;

        public bool HasDurationOfTwoHoursTops() => DurationInMinutes <= TimeSpan.FromHours(2);

        public bool StartsInTwentyFourHoursOrLess(ICurrentDate currentDate) => InitialDate <= currentDate.DateUtcNow().AddHours(24);
    };
}
