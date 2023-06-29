namespace CarReservation.Api.Models.Domain
{
    public record Reservation(Guid Id, string CarId, DateTime InitialDate, TimeSpan DurationInMinutes) 
    {
        public DateTime EndDate { get; } = InitialDate + DurationInMinutes;

        public bool StartsInTwentyFourHoursOrLess() => InitialDate <= DateTime.UtcNow.AddHours(24);

        public bool HasDurationOfTwoHoursTops() => DurationInMinutes <= TimeSpan.FromHours(2);
    };
}
