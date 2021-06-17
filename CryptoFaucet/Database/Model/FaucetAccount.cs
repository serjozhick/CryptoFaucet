using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CryptoFaucet.Database.Model
{
    [Table("FaucetAccount")]
    public class FaucetAccount
    {
        [Key]
        public string AccountId { get; set; } 

        public decimal BtcBalance { get; set; }

        public virtual ICollection<FaucetAccountTransaction> Transactions { get; set; }

        public FaucetAccount()
        {
            Transactions = new List<FaucetAccountTransaction>();
        }
    }
}