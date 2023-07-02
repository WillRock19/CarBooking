using CarReservation.Api.Interfaces;
using CarReservation.Api.Interfaces.Repositories;
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

        public IEnumerable<Reservation> GetAllUpcomingReservations(ICurrentDate currentDate) =>
            Database.Values
                .Where(x => x.InitialDate > currentDate.DateUtcNow());

        public IEnumerable<string> FindCarsReservedDuringDate(DateTime desiredDate) =>
                Database.Values
                     .Where(x => x.InitialDate <= desiredDate && desiredDate <= x.EndDate)
                     .Select(x => x.CarId);
    }
}
