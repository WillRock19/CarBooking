﻿using AutoMapper;
using CarReservation.Api.Interfaces;
using CarReservation.Api.Models.Domain;
using CarReservation.Api.Models.DTO.Request;
using CarReservation.Api.Models.DTO.Response;
using FluentValidation;
using FluentValidation.Results;
using System.Text;

namespace CarReservation.Api.Services
{
    public class CarService : ICarService
    {
        private readonly ICarRepository carRepository;
        private readonly IMapper mapper;
        private readonly IReservationRepository reservationRepository;
        private readonly IValidator<Reservation> reservationValidator;

        public CarService(ICarRepository carRepository, IMapper mapper, IReservationRepository reservationRepository, IValidator<Reservation> reservationValidator)
        {
            this.carRepository = carRepository;
            this.mapper = mapper;
            this.reservationRepository = reservationRepository;
            this.reservationValidator = reservationValidator;
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

        public async Task<(int? reservationId, string errorMessage)> ReserveCarAsync(ReservationRequest reservationRequest)
        {
            var reservation = mapper.Map<Reservation>(reservationRequest);

            var validationResult = await reservationValidator
                .ValidateAsync(reservation);
            
            if(!validationResult.IsValid)
                return (null, BuildReservationValidErrorMessage(validationResult.Errors));

            var carsReservedInIntervalSet = new HashSet<string>(reservationRepository
                .FindCarsReservedInInterval(reservation.InitialDate, reservation.EndDate));

            var carsAvailableForReservation = carRepository
                .GetAll()
                .Where(car => !carsReservedInIntervalSet.Contains(car.Id))
                .ToList();

            if (!carsAvailableForReservation.Any()) 
                return (null, "There's no car available for the desired date and time.");

            var carToReserve = carsAvailableForReservation.First();
            var reservationId = reservationRepository.Add(reservation with { CarId = carToReserve.Id });

            return (reservationId, string.Empty);
        }

        private string BuildReservationValidErrorMessage(List<ValidationFailure> validationFailures) 
        {
            var errorMessage = new StringBuilder();
            errorMessage.AppendLine("The reservation cannot be made. The following errors occured: ");

            validationFailures.ForEach(failure => errorMessage.AppendLine($"* {failure.ErrorMessage}"));
            return errorMessage.ToString();
        }
    }
}
