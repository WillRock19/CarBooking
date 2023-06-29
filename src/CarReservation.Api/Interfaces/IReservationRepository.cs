using CarReservation.Api.Models.Domain;

namespace CarReservation.Api.Interfaces
{
    public interface IReservationRepository
    {
        int Add(Reservation entity);

        Reservation? GetById(int reservationId);

        IEnumerable<string> FindCarsReservedInInterval(DateTime reservationBeginning, DateTime reservationEnding);
    }
}
