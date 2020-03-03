# CrappyPrizm

Library for working with the pseudo-cryptocurrency PRIZM. Designed specifically for unlucky developers forced to face this sh*t

-------------

Why is it a fraud?<br>
Why is it a financial pyramid?<br>
Why is it not even a cryptocurrency?<br>

I'm too lazy to explain, so I'll just leave a link to their [repository][crappy-repo] *with partially closed source code...*

Partially<br>
Сlosed<br>
Source<br>
Cryptocurrency...<br>
CRYPTOCURRENCY!

AHAHAHAHAAHAHAHAHAHAHAHAH

-----

## Code example

```csharp
// ApiClient initialization
IPrizmClient client = new PrizmClient();


// Accessing account's methods
IAccountEndpoint accountEndpoint = client.Accounts;
accountEndpoint = new AccountEndpoint(client);
accountEndpoint = client.Get<IAccountEndpoint>();
accountEndpoint = client.Get<AccountEndpoint>();

// Receiving wallet information (address, id and balance)
Account account = await accountEndpoint.GetAccountAsync("PRIZM-TE8N-B3VM-JJQH-5NYJB");

// New wallet generation
account = accountEndpoint.CreateAccount();


// Accessing transactions' methods
ITransactionEndpoint transactionEndpoint = client.Transactions;
transactionEndpoint = new TransactionEndpoint(client);
transactionEndpoint = client.Get<ITransactionEndpoint>();
transactionEndpoint = client.Get<TransactionEndpoint>();

// Getting all transactions for the specified wallet
IAsyncEnumerable<Transaction> transactions = transactionEndpoint.GetTransactionsAsync("PRIZM-TE8N-B3VM-JJQH-5NYJB");

// Funds' transafer
Transaction transaction = await transactionEndpoint.SendAsync("sender's secret phrase", "recipient's public key", 42);

// Transferring all funds to another wallet
transaction = await transactionEndpoint.SendAllAsync("sender's secret phrase", "recipient's public key");
```


[crappy-repo]: https://github.com/prizmspace/PrizmCore