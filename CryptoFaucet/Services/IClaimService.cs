using System.Threading.Tasks;

namespace CryptoFaucet.Services
{
    public interface IClaimService
    {
         Task<ClaimStatus> TryClaim(string email);
    }
}