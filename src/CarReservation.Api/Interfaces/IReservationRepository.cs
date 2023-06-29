using CarReservation.Api.Models.Domain;

namespace CarReservation.Api.Interfaces
{
    public interface IReservationRepository
    {
        Guid Add(Reservation entity);

        Reservation? GetById(Guid id);

        IEnumerable<string> FindCarsReservedInInterval(DateTime reservationBeginning, DateTime reservationEnding);
    }
}
