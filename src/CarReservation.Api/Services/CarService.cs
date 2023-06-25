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

        public CarResponse? GetCar(string carId)
        {
            var car = carRepository.GetById(carId);

            if (car == null) return null;

            return mapper.Map<CarResponse>(car);
        }

        public string AddCar(CarRequest carRequest)
        {
            var carEntity = mapper.Map<Car>(carRequest);
            return carRepository.Add(carEntity);
        }

        public CarResponse UpdateCar(string carId, CarRequest carRequest)
        {
            var _ = carRepository.GetById(carId) ?? throw new KeyNotFoundException($"Operation cannot be completed. There's no car with {carId}");
            var carUpdated = carRepository.Update(mapper.Map<Car>(carRequest) with 
            { 
                Id = carId 
            });

            return mapper.Map<CarResponse>(carUpdated);
        }

        public void DeleteCar(string carId) => carRepository.Delete(carId);
    }
}
