using System.Threading.Tasks;
using CryptoFaucet.Database.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace CryptoFaucet.Database
{
    public interface IFaucetDbContext
    {
        DbSet<FaucetAccount> Accounts { get; set; }
        DbSet<FaucetAccountTransaction> AccountTransactions { get; set; }
        DbSet<FaucetClaim> Claims { get; set; }

        Task SaveChangesAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}