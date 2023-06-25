using CarReservation.Api.Interfaces;
using CarReservation.Api.Models.Domain;

namespace CarReservation.Api.Repositories
{
    public class CarRepository : ICarRepository
    {
        private int LatestRegisterCount;
        private readonly Dictionary<string, Car> Database;

        public CarRepository()
        {
            LatestRegisterCount = 0;
            Database = new();
        }

        public IEnumerable<Car> GetAll() => Database.Values;

        public Car? GetById(string carId)
        {
            Database.TryGetValue(carId, out var entity);
            return entity;
        }

        public string Add(Car entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            var updatedLatestRegisterCount = LatestRegisterCount + 1;
            var entityId = Car.CreateId(updatedLatestRegisterCount);

            Database.Add(entityId, entity with { Id = entityId });
            LatestRegisterCount = updatedLatestRegisterCount;

            return entityId;
        }

        public void Delete(string carId)
        {
            if (!Database.ContainsKey(carId))
                throw new KeyNotFoundException($"Operation cannot be completed. There's no card with id '{carId}'.");

            Database.Remove(carId);
        }
    }
}
