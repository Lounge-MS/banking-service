using Microsoft.AspNetCore.Mvc;

namespace BankingServiceCallbackHandlerProject;

[ApiController]
public class CallbackHandlerController : ControllerBase
{
    private readonly CallbackHandlerService _service;

    public CallbackHandlerController(
        CallbackHandlerService service)
    {
        _service = service;
    }

    [HttpPost("/reply/{providerTypeName}/{paymentId}")]
    public async Task<IActionResult> GetMockReplyMessageAsync(
        string providerTypeName,
        string paymentId,
        CancellationToken cancellationToken = default)
    {
        await _service.GetMessageAsync(paymentId, Request, providerTypeName, cancellationToken);
        return Ok();
    }
}