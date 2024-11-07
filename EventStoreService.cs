using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using EventStore.Client;

namespace BankAccount;

public  class EventStoreService
{
    private EventStoreClient? _client;

    private EventStoreClient GetClient()
    {
        if (_client != null)
            return _client;
        var settings = EventStoreClientSettings.Create("esdb://localhost:2113?tls=false");
        _client = new EventStoreClient(settings);
        return _client;
    }


    public async Task AppendToStreamAsync<TEvent>(string streamName, TEvent @event)
    {
        var eventData = new EventData(
            Uuid.NewUuid(),
            @event.GetType().Name,
            JsonSerializer.SerializeToUtf8Bytes(@event)
        );
        
        await GetClient().AppendToStreamAsync(
            streamName,
            StreamState.Any,
            [eventData],
            cancellationToken: default
        );
    }

    public async Task<IEnumerable<string>> ReadFromStreamAsync<T>(string streamName)
    where T : Event
    {
        try
        {
            var streamResult = GetClient().ReadStreamAsync(Direction.Forwards, streamName, StreamPosition.Start);
            var eventStream = await streamResult.Where(x => x.Event.EventType == typeof(T).Name).ToListAsync();
            return eventStream.Select(resolved => Encoding.UTF8.GetString(resolved.Event.Data.ToArray())).ToList();
        }
        catch (Exception e)
        {
            //todo can log exception
            return [];
        }
    }

}