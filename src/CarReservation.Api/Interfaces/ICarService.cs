using CarReservation.Api.Models.DTO.Request;
using CarReservation.Api.Models.DTO.Response;

namespace CarReservation.Api.Interfaces
{
    public interface ICarService
    {
        IEnumerable<CarResponse> GetAllCars();

        string InsertCar(CarRequest carRequest);
    }
}
