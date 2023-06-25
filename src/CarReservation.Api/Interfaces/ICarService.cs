using CarReservation.Api.Models.DTO.Request;
using CarReservation.Api.Models.DTO.Response;

namespace CarReservation.Api.Interfaces
{
    public interface ICarService
    {
        IEnumerable<CarResponse> GetAllCars();

        CarResponse GetCar(string carId);

        string AddCar(CarRequest carRequest);

        CarResponse UpdateCar(CarRequest carRequest);

        void DeleteCar(string carId);
    }
}
