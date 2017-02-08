using System;
using System.Linq;
using System.Threading.Tasks;
using CommonDomain;
using CommonDomain.Persistence;
using NEventStore;
using RobotWar.Contracts;
using RobotWar.Domain;
using SB.Betting.Utilities;

namespace RobotWar.Infrastructure.EventStore
{
    public class RobotWarRepository : IRobotWarRepository
    {
        private readonly IRepository _repository;
        private readonly IStoreEvents _store;

        public RobotWarRepository(IRepository repository, IStoreEvents store)
        {
            _repository = ArgCheck.IsNotNull(repository, nameof(repository));
            _store = ArgCheck.IsNotNull(store, nameof(store));
        }

        public void Save(IAggregate aggregate)
        {
            var takeSnapshot = aggregate.GetUncommittedEvents().Cast<IEvent>().Any(p => p.Version > 0 && p.Version % 5 == 0);
            _repository.Save(aggregate, Guid.NewGuid());
            if (!takeSnapshot) return;

            var aggregateCopy = ((RobotWarAggregate)aggregate).Clone() as IAggregate;
            if (aggregateCopy == null)
            {
                throw new ApplicationException("Something wrong while cloning the aggregate");
            }
            Task.Run(() =>
            {
                _store.Advanced.AddSnapshot(new Snapshot(aggregateCopy.Id.ToString(), aggregateCopy.Version, RobotWarMemento.Create(aggregateCopy)));
            });
        }

        public RobotWarAggregate Read(Guid id)
        {
            return _repository.GetById<RobotWarAggregate>(id);
        }
    }
}