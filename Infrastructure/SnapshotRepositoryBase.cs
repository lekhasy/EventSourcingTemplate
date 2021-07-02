using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using EventStore.Client;

namespace EventStorePlayGround.Infrastructure
{
    public abstract class SnapshotRepositoryBase<AggregateType, AggregateStateType>
    where AggregateType : SnapshotAggregateBase<AggregateStateType>
    where AggregateStateType : IAggregateState
    {
        private EventStoreClient client { get; init; }

        public SnapshotRepositoryBase()
        {
            var settings = EventStoreClientSettings.Create("esdb://localhost:2113?tls=false");
            client = new EventStoreClient(settings);
        }

        protected abstract Dictionary<string, Type> EventTypeMap { get; }

        public abstract AggregateType GetDefaultAggregate();

        protected virtual string GetSnapshotStreamName(string id)
        {
            return GetStreamName(id) + "_snap";
        }
        protected virtual string GetStreamName(string id)
        {
            // one stream per aggregate
            return id;
        }

        public async Task<AggregateType> GetAsync(string id)
        {
            var snapshot = await GetSnapshotAsync(id);

            var result = client.ReadStreamAsync(Direction.Forwards, GetStreamName(id), StreamPosition.FromInt64(snapshot.Version + 1));

            var aggregate = GetDefaultAggregate();
            aggregate.SetState(snapshot.State);

            await foreach (var ev in result)
            {
                var EventBase = DeserializeEvent(ev.Event);
                aggregate.ApplyEvent(EventBase);
            }

            return aggregate;
        }

        private EventBase DeserializeEvent(EventRecord evnt)
        {
            var expectedType = EventTypeMap[evnt.EventType];
            return JsonSerializer.Deserialize(evnt.Data.Span, expectedType) as EventBase;
        }

        protected async Task<Snapshot<AggregateStateType>> GetSnapshotAsync(string id)
        {
            var result = client.ReadStreamAsync(Direction.Backwards, GetSnapshotStreamName(id), StreamPosition.End, 1);

            EventRecord latestSnapshotEvent = null;

            await foreach (var ev in result)
            {
                latestSnapshotEvent = ev.Event;
            }

            if (latestSnapshotEvent != null)
            {
                return JsonSerializer.Deserialize<Snapshot<AggregateStateType>>(latestSnapshotEvent.Data.Span);
            }
            else
            {
                return new Snapshot<AggregateStateType>()
                {
                    Version = -1,
                    State = GetDefaultAggregate().GetState()
                };
            }
        }
    }

    public class Snapshot<AggregateStateType>
    {
        public long Version { get; set; }
        public AggregateStateType State { get; set; }
    }

}
