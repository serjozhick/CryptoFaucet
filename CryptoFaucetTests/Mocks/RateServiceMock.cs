using CryptoFaucet.Services;
using System;
using System.Threading.Tasks;

namespace CryptoFaucetTests.Mocks
{
    public class RateServiceMock : IExchangeRateService
    {
        private decimal? rate = 1M;
        public void Setup(decimal? rate)
        {
            this.rate = rate;
        }
        public Task<decimal> GetBtcInUsd(uint expiration = 300000)
        {
            if (rate != null)
            {
                return Task.FromResult(rate.Value);
            }
            throw new Exception("Service unavailable");
        }
    }
}
