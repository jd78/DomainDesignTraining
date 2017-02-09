using System;
using CommonDomain;
using RobotWar.Domain;

namespace RobotWar.Infrastructure.EventStore
{
    public interface IRobotWarRepository
    {
        void Save(IAggregate aggregate);
        RobotWarAggregate Read(Guid id);
    }
}