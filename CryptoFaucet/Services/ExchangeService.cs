using System.Threading.Tasks;

namespace CryptoFaucet.Services
{
    public class ExchangeService : IExchangeService
    {
        private IExchangeRateService RateService { get; }
        
        public ExchangeService(IExchangeRateService rateService)
        {
            RateService = rateService;
        }

        private async Task<decimal> GetRate(uint? rateExpiration)
        {
            return rateExpiration != null ?
                await RateService.GetBtcInUsd(rateExpiration.Value) :
                await RateService.GetBtcInUsd();
        }

        public async Task<decimal> UsdToBtc(decimal usdValue, uint? rateExpiration = null)
        {
            var rate = await GetRate(rateExpiration);
            return usdValue / rate;
        }

        public async Task<decimal> BtcToUsd(decimal btcValue, uint? rateExpiration = null)
        {
            var rate = await GetRate(rateExpiration);
            return btcValue * rate;
        }
    }
}