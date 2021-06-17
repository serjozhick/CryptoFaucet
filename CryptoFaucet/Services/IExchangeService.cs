using System.Threading.Tasks;

namespace CryptoFaucet.Services
{
    public interface IExchangeService
    {
        Task<decimal> UsdToBtc(decimal usdValue, uint? rateExpiration = null);

        Task<decimal> BtcToUsd(decimal btcValue, uint? rateExpiration = null);
    }
}