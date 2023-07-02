using CarReservation.Api.Models.DTO.Request;
using CarReservation.Api.Models.DTO.Response;

namespace CarReservation.Api.Interfaces.Infraestructure
{
    public interface ICarService
    {
        IEnumerable<CarResponse> GetAllCars();

        CarResponse? GetCar(string carId);

        string AddCar(CreateCarRequest carRequest);

        CarResponse UpdateCar(string carId, CreateCarRequest carRequest);

        void DeleteCar(string carId);

        Task<CreateReservationResponse> ReserveCarAsync(CreateReservationRequest reservationRequest);

        IEnumerable<ReservationResponse> GetAllUpcomingReservationsUntil(DateTime? limitDate);
    }
}
