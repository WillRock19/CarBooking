using CarReservation.Api.Models.Domain;

namespace CarReservation.Api.Interfaces.Repositories
{
    public interface IReservationRepository
    {
        Guid Add(Reservation entity);

        Reservation? GetById(Guid id);

        IEnumerable<Reservation> GetAll();

        IEnumerable<string> FindCarsReservedDuringDate(DateTime desiredDate);
    }
}
