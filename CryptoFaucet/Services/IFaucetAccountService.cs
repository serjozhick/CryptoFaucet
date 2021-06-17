using System.Threading.Tasks;
using CryptoFaucet.Dto;

namespace CryptoFaucet.Services
{
    public interface IFaucetAccountService
    {
        Task<FaucetBalance> GetBalance();
        Task RefillAccount(decimal usdAmount);
        Task<bool> Withdraw(decimal btcAmount);
    }
}