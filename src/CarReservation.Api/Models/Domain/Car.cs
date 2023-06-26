namespace CarReservation.Api.Models.Domain
{
    public record Car(string Id, string Make, string Model) 
    {
        public static string CreateId(long number) => $"C{number}";
    }
}
