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
    public class ClaimTests : IClassFixture<TestFixture<Startup>>
    {
        private HttpClient client;
        public TestServer server;

        public ClaimTests(TestFixture<Startup> fixture)
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
            var accounts = new List<FaucetAccount>();
            db.accountsMock = accounts.AsQueryable().BuildMockDbSet();

            // Act
            var response = await client.GetAsync(request);

            // Assert
            response.EnsureSuccessStatusCode();
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<short>(jsonResponse);
            Assert.Equal((short)ClaimStatus.Failed, result);
            Assert.Single(db.SavedClaims);
            Assert.Equal((short)ClaimStatus.Failed, db.SavedClaims[0].Status);


            // Arrange no balance
            accounts.Add(new FaucetAccount
            {
                AccountId = FaucetAccountService.ACCOUNT_ID,
                BtcBalance = 0
            });

            // Act
            response = await client.GetAsync(request);

            // Assert
            response.EnsureSuccessStatusCode();
            jsonResponse = await response.Content.ReadAsStringAsync();
            result = JsonConvert.DeserializeObject<short>(jsonResponse);
            Assert.Equal((short)ClaimStatus.NotEnoughValue, result);
            Assert.Equal(2, db.SavedClaims.Count);
            Assert.Equal((short)ClaimStatus.NotEnoughValue, db.SavedClaims[1].Status);


            // Arrange filled balance
            accounts[0].BtcBalance = 4;

            // Act
            response = await client.GetAsync(request);

            // Assert
            response.EnsureSuccessStatusCode();
            jsonResponse = await response.Content.ReadAsStringAsync();
            result = JsonConvert.DeserializeObject<short>(jsonResponse);
            Assert.Equal((short)ClaimStatus.Success, result);
            Assert.Equal(3, db.SavedClaims.Count);
            Assert.Equal((short)ClaimStatus.Success, db.SavedClaims[2].Status);
            Assert.Equal(3.999M, db.SavedAccounts[0].BtcBalance);


            // Act second try
            response = await client.GetAsync(request);

            // Assert
            response.EnsureSuccessStatusCode();
            jsonResponse = await response.Content.ReadAsStringAsync();
            result = JsonConvert.DeserializeObject<short>(jsonResponse);
            Assert.Equal((short)ClaimStatus.TooFrequent, result);
            Assert.Equal(4, db.SavedClaims.Count);
            Assert.Equal((short)ClaimStatus.TooFrequent, db.SavedClaims[3].Status);
            Assert.Equal(3.999M, db.SavedAccounts[0].BtcBalance);
        }
    }
}
