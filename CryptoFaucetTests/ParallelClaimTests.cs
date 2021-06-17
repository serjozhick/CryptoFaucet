using CryptoFacet;
using CryptoFaucet.Database;
using CryptoFaucet.Database.Model;
using CryptoFaucet.Services;
using CryptoFaucetTests.Mocks;
using Microsoft.AspNetCore.TestHost;
using MockQueryable.Moq;
using Moq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace CryptoFaucetTests
{
    public class ParallelClaimTests : IClassFixture<TestFixture<Startup>>
    {
        private HttpClient client;
        public TestServer server;

        public ParallelClaimTests(TestFixture<Startup> fixture)
        {
            client = fixture.Client;
            server = fixture.Server;
        }

        [Fact]
        public async Task TestGetBalanceAsync()
        {
            // Arrange no account
            var request = "faucet/claim/test@company.ee";
            var db = server.Services.GetService(typeof(IFaucetDbContext)) as DbMock;
            var claims = new List<FaucetClaim>();
            db.claimsMock = claims.AsQueryable().BuildMockDbSet();
            db.claimsMock.Setup(c => c.Add(It.IsAny<FaucetClaim>())).Callback<FaucetClaim>(r => {
                claims.Add(r);
            });
            var accounts = new List<FaucetAccount>(new[]
            {
                new FaucetAccount
                {
                    AccountId = FaucetAccountService.ACCOUNT_ID,
                    BtcBalance = 5
                }
            });
            db.accountsMock = accounts.AsQueryable().BuildMockDbSet();
            (server.Services.GetService(typeof(IExchangeRateService)) as RateServiceMock).Setup(100);

            // Act
            var accountService = server.Services.GetService(typeof(IFaucetAccountService)) as IFaucetAccountService;
            await Task.WhenAll(
                client.GetAsync(request),
                accountService.RefillAccount(400),
                client.GetAsync(request));

            //// Assert
            Assert.Equal(2, db.SavedClaims.Count);
            Assert.Equal(8.999M, accounts[0].BtcBalance);
        }
    }
}
