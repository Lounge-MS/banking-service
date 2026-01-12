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

    [HttpPost("/mock/reply")]
    public async Task<IActionResult> GetMockReplyMessageAsync(
        CancellationToken cancellationToken = default)
    {
        await _service.GetMessageAsync(Request, cancellationToken);
        return Ok();
    }
}