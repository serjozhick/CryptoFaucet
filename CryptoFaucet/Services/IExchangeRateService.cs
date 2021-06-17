using System.Threading.Tasks;

namespace CryptoFaucet.Services
{
    public interface IExchangeRateService
    {
        Task<decimal> GetBtcInUsd(uint expiration = 300000);
    }
}