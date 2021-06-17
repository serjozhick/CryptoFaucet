using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CryptoFaucet.Database.Model
{
    [Table("FaucetClaim")]
    public class FaucetClaim
    {
        [Key]
        public string ClaimId { get; set; }

        public string Email { get; set; }

        public DateTime ClaimTime { get; set; }

        public short Status { get; set; }
    }
}