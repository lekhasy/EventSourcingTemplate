
using System;
using System.Collections.Generic;
using EventStorePlayGround.Infrastructure;

namespace EventStorePlayGround
{
    class UserBalanceRepository : SnapshotRepositoryBase<UserBalanceAggregate, UserBalanceAggregateState>
    {
        protected override Dictionary<string, Type> EventTypeMap { get => UserBalanceEvents.EventTypeMap; }

        public override UserBalanceAggregate GetDefaultAggregate()
        {
            return new UserBalanceAggregate();
        }
    }

}
