namespace CarReservation.Api.Models.Domain
{
    public record Reservation(int Id, string CarId, DateTime InitialDate, int DurationInMinutes) 
    {
        public DateTime EndDate = InitialDate.AddMinutes(DurationInMinutes);

        public bool StartsInTwentyFourHoursOrLess() => InitialDate <= DateTime.UtcNow.AddHours(24);

        public bool HasDurationOfTwoHoursTops() => DurationInMinutes <= 120 && EndDate <= InitialDate.AddHours(2);
    };
}
