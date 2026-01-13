using BankingServiceProject.Domain;
using Microsoft.AspNetCore.Mvc;

namespace BankingServiceProject;

[ApiController]
public class BankingServiceController : ControllerBase
{
    private readonly BankingService _service;

    public BankingServiceController(
        BankingService service)
    {
        _service = service;
    }

    [HttpPost("/reply/{providerTypeName}/{paymentId}")]
    public async Task<IActionResult> GetMockReplyMessageAsync(
        string providerTypeName,
        string paymentId,
        CancellationToken cancellationToken = default)
    {
        ParsedRequest parsedRequest = await ParsedRequest.ParseRequestAsync(Request);
        await _service.ReceivePaymentResultAsync(paymentId, parsedRequest, providerTypeName, cancellationToken);
        return Ok();
    }
}