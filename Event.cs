namespace BankAccount;


public class Event
{
    public DateTime OccuredAt { get; set; }
    public Guid EventId { get; set; }
}


public class DepositEvent : Event
{
    public decimal Amount { get; set; }
}

public class WithdrawalEvent : Event
{
    public decimal Amount { get; set; }
}
