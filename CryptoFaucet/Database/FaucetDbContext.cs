using CryptoFaucet.Database.Model;
using CryptoFaucet.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Threading.Tasks;

namespace CryptoFaucet.Database
{
    public class FaucetDbContext : DbContext, IFaucetDbContext
    {
        public FaucetDbContext(DbContextOptions<FaucetDbContext> options)
            : base(options)
        {
        }

        public DbSet<FaucetAccount> Accounts { get; set; }
        public DbSet<FaucetAccountTransaction> AccountTransactions { get; set; }
        public DbSet<FaucetClaim> Claims { get; set; }

        public Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return Database.BeginTransactionAsync();
        }

        public Task SaveChangesAsync()
        {
            SaveChanges();
            return Task.CompletedTask;
        }
    }
}