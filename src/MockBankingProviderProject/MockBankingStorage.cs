using BankingServiceProject.MockBankingProviderProject.Domain;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace BankingServiceProject.MockBankingProviderProject;

public class MockBankingStorage
{
    private readonly ConcurrentDictionary<string, MockPayment> _payments = new();
    private readonly MockBankingOptions _options;
    private readonly IHttpClientFactory _clientFactory;

    public MockBankingStorage(
        IOptions<MockBankingOptions> options,
        IHttpClientFactory httpClientFactory)
    {
        _options = options.Value;
        _clientFactory = httpClientFactory;
    }

    public ValueTask StartPayment(
        decimal amount,
        Uri confirmationUrl,
        Uri cancellationUrl,
        string identityToken,
        CancellationToken cancellationToken = default)
    {
        var payment = new MockPayment(amount, confirmationUrl, cancellationUrl, identityToken);
        _payments[payment.Id] = payment;

        if (!_options.RequireUrlVisit)
        {
            _ = ExecutePayment(payment, cancellationToken);
        }

        return ValueTask.CompletedTask;
    }

    private async Task ExecutePayment(
        MockPayment payment,
        CancellationToken cancellationToken = default)
    {
        await Task.Delay(_options.DelayMs, cancellationToken);
        if (_options.AllowPayment)
        {
            await AcceptPayment(payment, cancellationToken);
        }
        else
        {
            await DeclinePayment(payment, cancellationToken);
        }
    }

    private async Task AcceptPayment(
        MockPayment payment,
        CancellationToken cancellationToken = default)
    {
        payment.Status = MockPaymentStatus.Approved;
        using HttpClient client = _clientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Identity-Token", payment.IdentityToken);
        await client.PostAsync(payment.ConfirmationUrl, null, cancellationToken);
    }

    private async Task DeclinePayment(
        MockPayment payment,
        CancellationToken cancellationToken = default)
    {
        payment.Status = MockPaymentStatus.Declined;
        await SendWebhook(payment.CancellationUrl, payment.IdentityToken, cancellationToken);
    }

    private async Task SendWebhook(
        Uri url,
        string identityToken,
        CancellationToken cancellationToken = default)
    {
        using HttpClient client = _clientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Identity-Token", identityToken);
        await client.PostAsync(url, null, cancellationToken);
    }
}