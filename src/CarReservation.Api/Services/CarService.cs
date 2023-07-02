using AutoMapper;
using CarReservation.Api.Interfaces;
using CarReservation.Api.Interfaces.Infraestructure;
using CarReservation.Api.Interfaces.Repositories;
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
        private readonly ICurrentDate currentDate;

        public CarService(
            ICarRepository carRepository, 
            ICurrentDate currentDate,
            IMapper mapper, 
            IReservationRepository reservationRepository, 
            IValidator<Reservation> reservationValidator)
        {
            this.carRepository = carRepository;
            this.currentDate = currentDate;
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

        public string AddCar(CreateCarRequest carRequest)
        {
            var carEntity = mapper.Map<Car>(carRequest);
            return carRepository.Add(carEntity);
        }

        public CarResponse UpdateCar(string carId, CreateCarRequest updatedCarRequest)
        {
            if(string.IsNullOrEmpty(carId))
                throw new ArgumentNullException(nameof(carId));

            if (updatedCarRequest == null)
                throw new ArgumentNullException(nameof(updatedCarRequest));

            var carUpdated = carRepository.Update(mapper.Map<Car>(updatedCarRequest) with 
            { 
                Id = carId 
            });

            return mapper.Map<CarResponse>(carUpdated);
        }

        public void DeleteCar(string carId) => carRepository.Delete(carId);

        public async Task<CreateReservationResponse> ReserveCarAsync(CreateReservationRequest reservationRequest)
        {
            var reservation = mapper.Map<Reservation>(reservationRequest);

            var validationResult = await reservationValidator
                .ValidateAsync(reservation);
            
            if(!validationResult.IsValid)
                return new CreateReservationResponse(BuildReservationValidErrorMessage(validationResult.Errors));

            var carsReservedInIntervalSet = new HashSet<string>(reservationRepository
                .FindCarsReservedDuringDate(reservation.InitialDate));

            var carsAvailableForReservation = carRepository
                .GetAll()
                .Where(car => !carsReservedInIntervalSet.Contains(car.Id))
                .ToList();

            if (!carsAvailableForReservation.Any()) 
                return new CreateReservationResponse("There's no car available for the desired date and time.");

            var carToReserve = carsAvailableForReservation.First();
            var reservationId = reservationRepository.Add(reservation with { CarId = carToReserve.Id });
            var successMessage = $"Reservation successfully created for {reservation.InitialDate}. Your reservation ID is: {reservationId}.";

            return new CreateReservationResponse(reservationId, carToReserve.Id, successMessage);
        }

        public IEnumerable<ReservationResponse> GetAllUpcomingReservationsUntil(DateTime? limitDate) 
        {
            var reservations = limitDate.HasValue 
                ? reservationRepository.GetAllUpcomingReservations(currentDate).Where(x => x.InitialDate <= limitDate.Value) 
                : reservationRepository.GetAllUpcomingReservations(currentDate);

            return mapper.Map<IEnumerable<ReservationResponse>>(reservations);
        }

        private string BuildReservationValidErrorMessage(List<ValidationFailure> validationFailures) 
        {
            var errorMessage = new StringBuilder("The reservation cannot be made. The following errors occured: ");

            validationFailures.ForEach(failure => errorMessage.Append($"\n* {failure.ErrorMessage}"));
            return errorMessage.ToString();
        }
    }
}
