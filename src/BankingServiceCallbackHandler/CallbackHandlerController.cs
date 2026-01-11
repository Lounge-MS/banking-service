using BankingServiceCallbackHandler.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Text;

namespace BankingServiceCallbackHandler;

[ApiController]
public class CallbackHandlerController : ControllerBase
{
    private readonly CallbackHandlerService _service;

    public CallbackHandlerController(
        CallbackHandlerService service)
    {
        _service = service;
    }

    [HttpPost("/reply")]
    public async Task<IActionResult> GetMessageAsync(CancellationToken cancellationToken = default)
    {
        if (!Request.Headers.TryGetValue("X-Identity-Token", out StringValues externalIdentityToken) ||
            string.IsNullOrWhiteSpace(externalIdentityToken))
        {
            throw new InvalidTokenException();
        }

        using var reader = new StreamReader(
           Request.Body,
           Encoding.UTF8,
           detectEncodingFromByteOrderMarks: false,
           bufferSize: 1024,
           leaveOpen: true);

        string rawRequestBody = await reader.ReadToEndAsync(cancellationToken);
        _service.GetMessage(externalIdentityToken.ToString(), rawRequestBody);
        return Ok();
    }
}