using BankingServiceProject.MockBankingProviderProject.Domain;
using BankingServiceProject.MockBankingProviderProject.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace BankingServiceProject.MockBankingProviderProject;

[ApiController]
public class MockBankingController : ControllerBase
{
    private readonly MockBankingProvider _provider;

    public MockBankingController(MockBankingProvider provider)
    {
        _provider = provider;
    }

    [HttpPost("start")]
    public async Task<IActionResult> StartPayment(
        [FromBody] StartPaymentRequest request)
    {
        return new OkObjectResult(await _provider.StartPayment(request));
    }

    [HttpPost("rollback")]
    public IActionResult RollbackPayment(
        [FromBody] StartRollbackRequest request)
    {
        if (!Request.Headers.TryGetValue("X-Identity-Token", out StringValues externalIdentityToken) ||
            string.IsNullOrWhiteSpace(externalIdentityToken))
        {
            throw new InvalidIdentityTokenException();
        }

        _provider.StartRollback(request, externalIdentityToken.ToString());
        return Ok();
    }

    [HttpGet("confirm/{paymentId}")]
    public async Task<IActionResult> ConfirmPayment(string paymentId)
    {
        return Content(await _provider.ConfirmPayment(paymentId));
    }
}