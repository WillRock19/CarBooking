using AutoMapper;
using CarReservation.Api.Interfaces;
using CarReservation.Api.Models.Domain;
using CarReservation.Api.Models.DTO.Request;
using CarReservation.Api.Models.DTO.Response;

namespace CarReservation.Api.Services
{
    public class CarService : ICarService
    {
        public readonly ICarRepository carRepository;
        public readonly IMapper mapper;

        public CarService(ICarRepository carRepository, IMapper mapper)
        {
            this.carRepository = carRepository;
            this.mapper = mapper;
        }

        public IEnumerable<CarResponse> GetAllCars()
        {
            var carsFromDatabase = carRepository.GetAll();
            return mapper.Map<IEnumerable<CarResponse>>(carsFromDatabase);
        }

        public CarResponse GetCar(string carId)
        {
            throw new NotImplementedException();
        }

        public string AddCar(CarRequest carRequest)
        {
            var carEntity = mapper.Map<Car>(carRequest);
            return carRepository.Add(carEntity);
        }

        public CarResponse UpdateCar(CarRequest carRequest)
        {
            throw new NotImplementedException();
        }

        public void DeleteCar(string carId)
        {
            throw new NotImplementedException();
        }
    }
}
