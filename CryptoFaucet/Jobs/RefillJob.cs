using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CryptoFaucet.Database;
using CryptoFaucet.Logging;
using CryptoFaucet.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CryptoFaucet.Jobs
{
    public class RefillJob : IHostedService
    {
        private const long refillFrequency = 3 * 60 * 60;
        private const decimal refillUsdAmount = 400;
        private CancellationTokenSource refillCancelation;
        private ILog Logger { get; }
        private IServiceScopeFactory ScopeFactory { get; }

        public RefillJob(
            ILog logger,
            IServiceScopeFactory scopeFactory)
        {
            Logger = logger;
            ScopeFactory = scopeFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            refillCancelation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            DateTime? LastTransactionTime = null;
            using (var scope = ScopeFactory.CreateScope())
            {
                var Db = scope.ServiceProvider.GetRequiredService<IFaucetDbContext>();
                LastTransactionTime = await Db.AccountTransactions
                    .OrderByDescending(transaction => transaction.TransactionTime)
                    .Select(Transaction => Transaction.TransactionTime)
                    .FirstOrDefaultAsync();
            }
            if (LastTransactionTime == null || DateTime.UtcNow.Subtract(LastTransactionTime.Value).TotalSeconds > refillFrequency)
            {
                await Refill(cancellationToken);
                StartPeriodicalRefill(TimeSpan.FromSeconds(refillFrequency), cancellationToken);
            }
            else
            {
                StartPeriodicalRefill(DateTime.UtcNow.Subtract(LastTransactionTime.Value), cancellationToken);
            }
        }

        private void StartPeriodicalRefill(TimeSpan initialDelay, CancellationToken cancellationToken)
        {
            Task.Run(async () =>
            {
                await Task.Delay(initialDelay, cancellationToken);
                await Refill(cancellationToken);
                while (!cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromSeconds(refillFrequency), cancellationToken);
                    await Refill(cancellationToken);
                }
            }, cancellationToken);
        }

        public async Task Refill(CancellationToken cancellationToken)
        {
            Logger.Info($"Start refill {DateTime.UtcNow}");
            if(!cancellationToken.IsCancellationRequested)
            {
                using (var scope = ScopeFactory.CreateScope())
                {
                    var AccountService = scope.ServiceProvider.GetRequiredService<IFaucetAccountService>();
                    await AccountService.RefillAccount(refillUsdAmount);
                }
                Logger.Info($"Account refilled for {refillUsdAmount}$.");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            refillCancelation.Cancel();

            return Task.CompletedTask;
        }
    }
}