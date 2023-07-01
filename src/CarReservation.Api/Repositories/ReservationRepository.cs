using CarReservation.Api.Interfaces;
using CarReservation.Api.Models.Domain;

namespace CarReservation.Api.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly Dictionary<Guid, Reservation> Database;

        public ReservationRepository()
        {
            Database = new();
        }

        public Guid Add(Reservation entity) 
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            var entityId = Guid.NewGuid();

            Database.Add(entityId, entity with { Id = entityId });
            return entityId;
        }

        public Reservation? GetById(Guid id)
        {
            Database.TryGetValue(id, out var entity);
            return entity;
        }

        public IEnumerable<Reservation> GetAll() => Database.Values;

        public IEnumerable<string> FindCarsReservedInInterval(DateTime reservationBeginning, DateTime reservationEnding) =>
             Database.Values
                     .Where(x => reservationBeginning <= x.InitialDate && x.EndDate <= reservationEnding)
                     .Select(x => x.CarId);  
    }
}
