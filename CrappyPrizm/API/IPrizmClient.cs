using CrappyPrizm.Endpoints;

namespace CrappyPrizm.API
{
    public interface IPrizmClient : IApiClient 
    {
        IAccountEndpoint Accounts { get; }

        ITransactionEndpoint Transactions { get; }
    }
}
