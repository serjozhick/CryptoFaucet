using CryptoFaucet.Database;
using CryptoFaucet.Database.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CryptoFaucetTests.Mocks
{
    public class DbMock : IFaucetDbContext
    {
        public Mock<DbSet<FaucetAccount>> accountsMock;
        public Mock<DbSet<FaucetAccountTransaction>> transactionsMock;
        public Mock<DbSet<FaucetClaim>> claimsMock;
        public DbSet<FaucetAccount> Accounts { get => accountsMock.Object; set => throw new NotImplementedException(); }
        public DbSet<FaucetAccountTransaction> AccountTransactions { get => transactionsMock.Object; set => throw new NotImplementedException(); }
        public DbSet<FaucetClaim> Claims { get => claimsMock.Object; set => throw new NotImplementedException(); }

        public IList<FaucetAccount> SavedAccounts { get; set; }
        public IList<FaucetAccountTransaction> SavedAccountTransactions { get; set; }
        public IList<FaucetClaim> SavedClaims { get; set; }

        public Task<IDbContextTransaction> BeginTransactionAsync()
        {
            var transactionMock = new Mock<IDbContextTransaction>();
            return Task.FromResult(transactionMock.Object);
        }

        public async Task SaveChangesAsync()
        {
            if(accountsMock != null)
            {
                SavedAccounts = (await Accounts.ToArrayAsync()).Clone() as FaucetAccount[];
            }
            if (transactionsMock != null)
            {
                SavedAccountTransactions = (await AccountTransactions.ToArrayAsync()).Clone() as FaucetAccountTransaction[];
            }
            if (claimsMock != null)
            {
                SavedClaims = (await Claims.ToArrayAsync()).Clone() as FaucetClaim[];
            }
        }
    }
}
