using BankingServiceProject.MockBankingProviderProject.Domain;
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
    public async Task<IActionResult> StartPayment([FromBody] StartPaymentRequest request)
    {
        return new OkObjectResult(await _provider.StartPayment(request));
    }

    [HttpPost("rollback")]
    public IActionResult RollbackPayment([FromBody] StartRollbackRequest request)
    {
        _provider.StartRollback(request);
        return Ok();
    }

    [HttpGet("confirm/{paymentId}")]
    public async Task<IActionResult> ConfirmPayment(string paymentId)
    {
        return Content(await _provider.ConfirmPayment(paymentId));
    }
}