using BankAccount;

var id = Guid.Parse("4f01486f-f1b7-44bc-989d-394afc628485");
var account = new BankAccount.BankAccount(id);



#region Simple

// account.Deposit(1000);
// account.Deposit(7000);
// account.Withdraw(100);
// account.ReplayEvents(account.GetEvents());
// Console.WriteLine($"balance is {account.GetBalance()}");

#endregion


#region SnapShot

var snapshot = new SnapShot(id, account.GetBalance());
await account.SaveSnapShot(snapshot);
var snapShots = account.GetSnapShotEvents();
var snapShotAsEvent = snapShots.Cast<Event>().ToList();
account.ReplayEvents(snapShotAsEvent);
Console.WriteLine($"balance of snapshots: {account.GetBalance()}");

#endregion


