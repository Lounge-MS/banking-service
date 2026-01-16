using BankingServiceProject.MockBankingProviderProject.Entities;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<IActionResult> StartPaymentAsync(
        [FromBody] StartPaymentRequest request)
    {
        return new OkObjectResult(await _provider.StartPaymentAsync(request));
    }

    [HttpGet("confirm/{paymentId}")]
    public async Task<IActionResult> ConfirmPaymentAsync(string paymentId)
    {
        return Content(await _provider.ConfirmPaymentAsync(paymentId));
    }
}