using BankingServiceProject.Entities.Dto;
using BankingServiceProject.Ports.Services;
using Microsoft.AspNetCore.Mvc;

namespace BankingServiceProject.PresentationLayerProject.Webhooks.Rest;

[ApiController]
public class BankingServiceController : ControllerBase
{
    private readonly IWebhookService _service;

    public BankingServiceController(
        IWebhookService service)
    {
        _service = service;
    }

    [HttpPost("/reply/{providerTypeName}/{paymentId}")]
    public async Task<IActionResult> GetMockReplyMessageAsync(
        string providerTypeName,
        string paymentId,
        CancellationToken cancellationToken = default)
    {
        ParsedRequest parsedRequest = await Request.ParseAsync(cancellationToken);
        await _service.ReceivePaymentResultAsync(paymentId, parsedRequest, providerTypeName, cancellationToken);
        return Ok();
    }
}