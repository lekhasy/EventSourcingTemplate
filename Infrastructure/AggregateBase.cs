using System;
using System.Collections.Generic;

namespace EventStorePlayGround.Infrastructure
{
    public abstract class AggregateBase
    {
        protected List<EventBase> UnCommitedEvents = new List<EventBase>();

        public abstract void ApplyEvent(EventBase events);
    }

    public interface IAggregateState
    {

    }

    public abstract class SnapshotAggregateBase<AggregateStateType> : AggregateBase where AggregateStateType : IAggregateState
    {
        public abstract AggregateStateType GetState();

        public abstract void SetState(AggregateStateType state);
    }
}
