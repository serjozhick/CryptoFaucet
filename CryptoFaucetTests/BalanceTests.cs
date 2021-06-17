using CryptoFacet;
using CryptoFaucet.Database;
using CryptoFaucet.Database.Model;
using CryptoFaucet.Dto;
using CryptoFaucet.Services;
using CryptoFaucetTests.Mocks;
using Microsoft.AspNetCore.TestHost;
using MockQueryable.Moq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace CryptoFaucetTests
{
    public class BalanceTests : IClassFixture<TestFixture<Startup>>
    {
        private HttpClient client;
        public TestServer server;

        public BalanceTests(TestFixture<Startup> fixture)
        {
            client = fixture.Client;
            server = fixture.Server;
        }

        [Fact]
        public async Task TestGetBalanceAsync()
        {
            // Arrange success
            var request = "faucet/balance";
            var db = server.Services.GetService(typeof(IFaucetDbContext)) as DbMock;
            var accounts = new List<FaucetAccount>(new[]
            {
                new FaucetAccount
                {
                    AccountId = FaucetAccountService.ACCOUNT_ID,
                    BtcBalance = 0.0005M
                }
            });

            db.accountsMock = accounts.AsQueryable().BuildMockDbSet();
            (server.Services.GetService(typeof(IExchangeRateService)) as RateServiceMock).Setup(400);

            // Act
            var response = await client.GetAsync(request);

            // Assert
            response.EnsureSuccessStatusCode();
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var balance = JsonConvert.DeserializeObject<FaucetBalance>(jsonResponse);
            Assert.Equal(0.0005M, balance.BtcValue);
            Assert.Equal(0.2M, balance.UsdValue);


            // Arrange no db entry
            accounts.Clear();

            // Act
            response = await client.GetAsync(request);

            // Assert
            Assert.False(response.IsSuccessStatusCode);


            // Arrange service error
            accounts.Add(new FaucetAccount
            {
                AccountId = FaucetAccountService.ACCOUNT_ID,
                BtcBalance = 10
            });
            (server.Services.GetService(typeof(IExchangeRateService)) as RateServiceMock).Setup(null);

            // Act
            response = await client.GetAsync(request);

            // Assert
            Assert.False(response.IsSuccessStatusCode);
        }
    }
}
