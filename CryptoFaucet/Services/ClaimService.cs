using System;
using System.Threading.Tasks;
using CryptoFaucet.Database;
using CryptoFaucet.Database.Model;
using CryptoFaucet.Logging;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;

namespace CryptoFaucet.Services {
    public class ClaimService : IClaimService {
        private const int FrequencyHours = 24;
        private const decimal ClaimAmount = 0.001M;
        private static SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
        private IFaucetDbContext Db { get; }
        private ILog Logger { get; }
        private IFaucetAccountService AccountService { get; }

        public ClaimService (
            IFaucetDbContext dbContext,
            ILog logger,
            IFaucetAccountService accountService)
        {
            Db = dbContext;
            Logger = logger;
            AccountService = accountService;
        }

        private async Task<FaucetClaim> GetCurrentClaim(string email)
        {
            await semaphore.WaitAsync();
            try
            {
                Logger.Info($"Try claim for {email}.");
                var currentClaim = new FaucetClaim
                {
                    ClaimId = Guid.NewGuid().ToString(),
                    ClaimTime = DateTime.UtcNow,
                    Status = (short)ClaimStatus.Unknown,
                    Email = email
                };
                Db.Claims.Add(currentClaim);
                await Db.SaveChangesAsync();
                return currentClaim;
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async Task<ClaimStatus> TryClaim(string email) {
            var currentClaim = await GetCurrentClaim(email);
            try
            {
                var claimThreashold = DateTime.UtcNow.AddHours(-FrequencyHours);
                var firstClaim = await Db.Claims
                    .Where(claim => claim.Email == email && claim.ClaimTime >= claimThreashold && (claim.Status == (short)ClaimStatus.Success || claim.Status == (short)ClaimStatus.Unknown))
                    .OrderBy(claim => claim.ClaimTime)
                    .FirstOrDefaultAsync();

                if(firstClaim.ClaimId == currentClaim.ClaimId)
                {
                    if(await AccountService.Withdraw(ClaimAmount))
                    {
                        currentClaim.Status = (short)ClaimStatus.Success;
                        await Db.SaveChangesAsync();
                        Logger.Info($"Calim was succedded for {email}.");
                    }
                    else
                    {
                        currentClaim.Status = (short)ClaimStatus.NotEnoughValue;
                        await Db.SaveChangesAsync();
                        Logger.Info($"Calim for {email} was not succedded. Not enough balance.");
                    }
                }
                else
                {
                    currentClaim.Status = (short)ClaimStatus.TooFrequent;
                    await Db.SaveChangesAsync();
                    Logger.Info($"Calim for {email} was not succedded. Too frequent. {firstClaim.ClaimTime}, {currentClaim.ClaimTime}");
                }
            }
            catch (Exception e)
            {
                currentClaim.Status = (short)ClaimStatus.Failed;
                await Db.SaveChangesAsync();
                Logger.Error($"Error on claim for {email}. {e}");
            }
            return (ClaimStatus)currentClaim.Status;
        }
    }
}