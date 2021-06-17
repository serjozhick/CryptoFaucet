using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CryptoFaucet.Database;
using CryptoFaucet.Database.Model;
using CryptoFaucet.Dto;
using CryptoFaucet.Logging;

namespace CryptoFaucet.Services
{
    public class FaucetAccountService : IFaucetAccountService
    {
        public const string ACCOUNT_ID = "396806e5-03f6-40ff-ae8f-2a5cb736dfe4";
        private static SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
        private IFaucetDbContext Db { get; }
        private ILog Logger { get; }
        private IExchangeService ExchangeService { get; }
        private IExchangeRateService RateService { get; }

        public FaucetAccountService(
            IFaucetDbContext dbContext,
            ILog logger,
            IExchangeService exchangeService,
            IExchangeRateService rateService)
        {
            Db = dbContext;
            Logger = logger;
            ExchangeService = exchangeService;
            RateService = rateService;
        }

        public async Task<FaucetBalance> GetBalance()
        {
            Logger.Info("Retrieving account balance.");
            var balance = await Db.Accounts
                .Where(account => account.AccountId == ACCOUNT_ID)
                .Select(account => new FaucetBalance
                    {
                        BtcValue = account.BtcBalance
                    })
                .FirstOrDefaultAsync();
            balance.UsdValue = await ExchangeService.BtcToUsd(balance.BtcValue);
            Logger.Info($"Balance is {balance.BtcValue} btc or {balance.UsdValue} usd.");
            return balance;
        }

        public async Task RefillAccount(decimal usdAmount)
        {
            uint expiration = 1000;
            var rate = await RateService.GetBtcInUsd(expiration);
            var btcAmount = await ExchangeService.UsdToBtc(usdAmount, expiration);
            Logger.Info($"Refilling account with {usdAmount}$ ({btcAmount} btc).");
            await semaphore.WaitAsync();
            try
            {
                var refillTransaction = await Db.BeginTransactionAsync();
                var account = await Db.Accounts
                    .Where(account => account.AccountId == ACCOUNT_ID)
                    .FirstOrDefaultAsync();
                account.Transactions.Add(new FaucetAccountTransaction
                {
                    BtcExchangeRate = rate,
                    CurrentBalance = account.BtcBalance + btcAmount,
                    InitialBalance = account.BtcBalance,
                    TransactionTime = DateTime.UtcNow,
                    AccountId = account.AccountId
                });
                account.BtcBalance += btcAmount;
                await Db.SaveChangesAsync();
                await refillTransaction.CommitAsync();
                Logger.Info($"Account refilled successfully.");
            }
            catch(Exception e)
            {
                Logger.Error($"Error refilling account. {e}");
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async Task<bool> Withdraw(decimal btcAmount)
        {
            Logger.Info($"Trying to withdraw {btcAmount} btc from account.");
            await semaphore.WaitAsync();
            try
            {
                var account = await Db.Accounts
                    .Where(account => account.AccountId == ACCOUNT_ID)
                    .FirstOrDefaultAsync();
                if (account.BtcBalance >= btcAmount)
                {
                    account.BtcBalance -= btcAmount;
                    await Db.SaveChangesAsync();
                    Logger.Info($"Account refilled successfully.");
                    return true;
                }
                else
                {
                    Logger.Info($"Account does not contain anough.");
                    return false;
                }
            }
            catch(Exception e)
            {
                Logger.Error($"Error withdrawing. {e}");
                throw;
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}