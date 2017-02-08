using CommonDomain.Core;
using RobotWar.Contracts;

namespace RobotWar.Domain
{
    public class DomainBase : AggregateBase
    {
        public virtual void RaiseDomainEvent(IEvent @event)
        {
            //@event.Id = Id;
            //@event.Version = Version;
            RaiseEvent(@event);
        }
    }
}
