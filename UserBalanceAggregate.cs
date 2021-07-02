
using System;
using System.Collections.Generic;
using EventStorePlayGround.Infrastructure;

namespace EventStorePlayGround
{
    class UserBalanceAggregate : SnapshotAggregateBase<UserBalanceAggregateState>
    {
        public UserBalance UserBalance { get; set; }

        public void Apply(UserBalanceEvents.Withdraw evnt)
        {
            UserBalance.Balance -= evnt.Amount;
        }

        public void Apply(UserBalanceEvents.Deposit envt)
        {
            UserBalance.Balance += envt.Amount;
        }

        public override void ApplyEvent(EventBase evnt)
        {
            switch (evnt)
            {
                case UserBalanceEvents.Withdraw ev:
                    {
                        Apply(ev);
                        break;
                    }
                case UserBalanceEvents.Deposit ev:
                    {
                        Apply(ev);
                        break;
                    }
                default: break;
            }
        }

        public override UserBalanceAggregateState GetState()
        {
            return new UserBalanceAggregateState
            {
                UserBalance = UserBalance
            };
        }

        public override void SetState(UserBalanceAggregateState state)
        {
            UserBalance = new UserBalance
            {
                Balance = state.UserBalance.Balance
            };
        }
    }

    class UserBalanceEvents
    {
        public class EventTypes
        {
            public static string Withdraw { get => "Withdraw"; }
            public static string Deposit { get => "Deposit"; }
        }

        public static Dictionary<string, Type> EventTypeMap = new Dictionary<string, Type>() {
            {EventTypes.Withdraw, typeof(UserBalanceEvents.Withdraw)}
        };

        public record Withdraw(decimal Amount) : EventBase;
        public record Deposit(decimal Amount) : EventBase;
    }

    class UserBalance
    {
        public decimal Balance { get; set; }
    }

    class UserBalanceAggregateState : IAggregateState
    {
        public UserBalance UserBalance { get; set; }
    }
}
