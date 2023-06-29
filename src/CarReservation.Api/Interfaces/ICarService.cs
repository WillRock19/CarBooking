using CarReservation.Api.Models.DTO.Request;
using CarReservation.Api.Models.DTO.Response;

namespace CarReservation.Api.Interfaces
{
    public interface ICarService
    {
        IEnumerable<CarResponse> GetAllCars();

        CarResponse? GetCar(string carId);

        string AddCar(CarRequest carRequest);

        CarResponse UpdateCar(string carId, CarRequest carRequest);

        void DeleteCar(string carId);

        Task<(int? reservationId, string errorMessage)> ReserveCarAsync(ReservationRequest reservationRequest);
    }
}
