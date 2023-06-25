using CarReservation.Api.Models.Domain;

namespace CarReservation.Api.Interfaces
{
    public interface ICarRepository
    {
        string AddCar(Car entity);

        Car? GetById(string carId);

        IEnumerable<Car> GetAll();

        void Delete(string carId);
    }
}
