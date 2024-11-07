namespace BankAccount;

public class SnapShot(Guid eventId, decimal balance) : Event
{
    private readonly DateTime snapshotDate = DateTime.UtcNow;

    public decimal SnapShotBalance  => balance;
}