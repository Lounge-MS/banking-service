using Microsoft.AspNetCore.Mvc;

namespace BankingServiceProject.ExternalConnectorProject;

[ApiController]
public class ExternalConnectorController : ControllerBase
{
    private readonly ExternalConnectorService _service;

    public ExternalConnectorController(
        ExternalConnectorService service)
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