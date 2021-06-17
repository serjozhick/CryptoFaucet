using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CryptoFaucet.Database.Model
{
    [Table("FaucetTransaction")]
    public class FaucetAccountTransaction
    {
        [Key]
        public DateTime TransactionTime { get; set;}

        public decimal InitialBalance { get; set; }

        public decimal CurrentBalance { get; set; }

        public decimal BtcExchangeRate { get; set; }

        [ForeignKey("FaucetAccount")]
        public string AccountId { get; set; }
        public FaucetAccount Account { get; set; }
    }
}