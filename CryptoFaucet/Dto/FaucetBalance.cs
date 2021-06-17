namespace CryptoFaucet.Dto
{
    /// <summary>
    /// Represents account balance.
    /// </summary>
    public class FaucetBalance
    {
        /// <summary>
        /// Balance in bitcoins.
        /// </summary>
        public decimal BtcValue { get; set; }

        /// <summary>
        /// Balance in us dollars.
        /// </summary>
        public decimal UsdValue { get; set; }
    }
}