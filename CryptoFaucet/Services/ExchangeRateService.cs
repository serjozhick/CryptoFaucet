using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace CryptoFaucet.Services
{
    public class ExchangeRateService : IExchangeRateService
    {
        private class CoinApiRateResponse
        {
            public decimal rate { get; set; }
        }
        private decimal cachedRate = 0;
        private DateTime refreshTime = DateTime.MinValue;

        public IConfiguration Configuration { get; }
        public ExchangeRateService(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public async Task<decimal> GetBtcInUsd(uint expiration = 300000)
        {
            if (DateTime.UtcNow.Subtract(refreshTime).TotalMilliseconds > expiration)
            {
                cachedRate = await RequestRate();
                refreshTime = DateTime.UtcNow;
            }
            return cachedRate;
        }

        private async Task<decimal>RequestRate()
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri("https://rest.coinapi.io/v1/exchangerate/BTC/USD"),
                Method = HttpMethod.Get,
                Headers = {
                    { "X-CoinAPI-Key", Configuration["CoinApiKey"]}
                }
            };
            using (var client = new HttpClient())
            {
                var httpResponse = await client.SendAsync(request);

                if (!httpResponse.IsSuccessStatusCode)
                {
                    throw new Exception($"Error reading rate. {httpResponse.StatusCode}");
                }

                var content = await httpResponse.Content.ReadAsStringAsync();
                var tasks = JsonConvert.DeserializeObject<CoinApiRateResponse>(content);

                return tasks.rate;
            }
        }
    }
}