using System.Text.Json;
using System.Text.Json.Serialization;
using EventStore.Client;

namespace BankAccount;

public class BankAccount(Guid id)
{
    private Guid Id { get; } = id;
    private decimal _balance = 0;
    private readonly EventStoreService _eventStoreService = new();
    private readonly string _streamName = id.ToString();

    public void Deposit(decimal amount)
    {
        var deposit = new DepositEvent
        {
            Amount = amount,
            EventId = Id,
            OccuredAt = DateTime.UtcNow
        };
        ApplyEvent(deposit);
        _eventStoreService.AppendToStreamAsync(_streamName, deposit);
    }

    public void Withdraw(decimal amount)
    {
        var withdrawal = new WithdrawalEvent
        {
            Amount = amount,
            EventId = Id,
            OccuredAt = DateTime.UtcNow
        };

        ApplyEvent(withdrawal);
        _eventStoreService.AppendToStreamAsync(_streamName, withdrawal);
    }

    public void ReplayEvents(List<Event> events)
    {
        foreach (var @event in events)
        {
            ApplyEvent(@event);
        }
    }

    public List<Event> GetEvents()
    {
        var result = new List<Event>();
        var deposit = GetEventsByType<DepositEvent>();
        var withDraw = GetEventsByType<WithdrawalEvent>();
        result.AddRange(deposit);
        result.AddRange(withDraw);
        return result;
    }

    
    
    
    public decimal GetBalance() => _balance;
    public async Task SaveSnapShot(SnapShot snapShot) =>await _eventStoreService.AppendToStreamAsync(id.ToString(), snapShot);

    public List<SnapShot> GetSnapShotEvents() => GetEventsByType<SnapShot>();

    
    private List<T> GetEventsByType<T>()
        where T : Event
    {
        var data = _eventStoreService.ReadFromStreamAsync<T>(_streamName).Result;

        return data.Select(@event => JsonSerializer.Deserialize<T>(@event)).OfType<T>().ToList();
    }


    private void ApplyEvent(Event @event)
    {
        switch (@event)
        {
            case DepositEvent depositEvent:
                _balance += depositEvent.Amount;
                break;
            case WithdrawalEvent withdrawalEvent:
                _balance -= withdrawalEvent.Amount;
                break;
            case SnapShot snapShot:
                _balance = snapShot.SnapShotBalance;
                break;
        }
    }
}