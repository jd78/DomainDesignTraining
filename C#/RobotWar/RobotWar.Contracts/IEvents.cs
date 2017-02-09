using System;

namespace RobotWar.Contracts
{
    public interface IEvent
    {
        Guid Id { get; }
        int Version { get; }
    }
}
