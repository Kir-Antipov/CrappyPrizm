using System.Numerics;
using CrappyPrizm.Models;
using System.Threading.Tasks;

namespace CrappyPrizm.Endpoints
{
    public interface IAccountEndpoint : IEndpoint
    {
        Account CreateAccount();

        Account CreateAccount(int secretLength);

        Account CreateAccount(string secretPhrase);

        Task<Account> GetAccountAsync(BigInteger accountId);

        Task<Account> GetAccountAsync(string address);
    }
}
