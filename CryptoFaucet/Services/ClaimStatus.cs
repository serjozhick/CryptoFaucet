namespace CryptoFaucet.Services
{
    /// <summary>
    /// Represents claim operation status.
    /// </summary>
    public enum ClaimStatus : short
    {
        /// <summary>
        /// Status of the claim is not yet determined.
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Claim was successfully processed.
        /// </summary>
        Success = 1,
        /// <summary>
        /// Claim operation failed.
        /// </summary>
        Failed = 2,
        /// <summary>
        /// Claim was rejected due to lack of the balance.
        /// </summary>
        NotEnoughValue = 3,
        /// <summary>
        /// Claim was rejected dut to request frequency.
        /// </summary>
        TooFrequent = 4
    }
}