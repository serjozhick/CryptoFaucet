using System.Threading.Tasks;
using CryptoFaucet.Logging;
using CryptoFaucet.Services;
using CryptoFaucet.Dto;
using Microsoft.AspNetCore.Mvc;

namespace CryptoFaucet.Controllers
{
    [Route("faucet")]
    [ApiController]
    public class FaucetController : ControllerBase
    {
        public IFaucetAccountService AccountService { get; }
        public IClaimService ClaimService { get; }

        public FaucetController(
            IFaucetAccountService accountService,
            IClaimService claimService)
        {
            AccountService = accountService;
            ClaimService = claimService;
        }

        /// <summary>
        /// Retrieves current account balance.
        /// </summary>
        /// <returns>Current balance as <c>FaucetBalance</c></returns>
        /// <response code="200">Returns current balance</response>
        /// <response code="500">Indicates service unavailability</response>  
        [HttpGet("balance")]
        public Task<FaucetBalance> GetBalance()
        {
            return AccountService.GetBalance();
        }

        /// <summary>
        /// Tries to claim asmall amount from the faucet.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /faucet/claim/name@company.ee
        ///
        /// </remarks>
        /// <param name="email"></param>
        /// <returns>Status of the claim operation</returns>
        [HttpGet("claim/{email}")]
        public Task<ClaimStatus> ClaimFaucet(string email)
        {
            return ClaimService.TryClaim(email);
        }
    }
}