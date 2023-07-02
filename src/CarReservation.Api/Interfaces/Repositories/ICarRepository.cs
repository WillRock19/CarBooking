using CarReservation.Api.Models.Domain;

namespace CarReservation.Api.Interfaces.Repositories
{
    public interface ICarRepository
    {
        string Add(Car entity);

        Car Update(Car entity);

        Car? GetById(string carId);

        IEnumerable<Car> GetAll();

        void Delete(string carId);
    }
}
