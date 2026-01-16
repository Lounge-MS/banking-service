using BankingServiceProject.MockBankingProviderProject.Entities;
using BankingServiceProject.MockBankingProviderProject.Entities.Payments;
using BankingServiceProject.MockBankingProviderProject.Exceptions;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;

namespace BankingServiceProject.MockBankingProviderProject;

public class MockBankingProvider
{
    private readonly ConcurrentDictionary<string, MockPayment> _payments = new();
    private readonly MockBankingOptions _options;
    private readonly IHttpClientFactory _clientFactory;
    private readonly JsonSerializerOptions _jsonOptions;

    public MockBankingProvider(
        IOptionsMonitor<MockBankingOptions> options,
        IHttpClientFactory httpClientFactory,
        JsonSerializerOptions jsonOptions)
    {
        _options = options.CurrentValue;
        _clientFactory = httpClientFactory;
        _jsonOptions = jsonOptions;
    }

    public ValueTask<StartPaymentResponse> StartPaymentAsync(
        StartPaymentRequest request,
        CancellationToken cancellationToken = default)
    {
        var payment = UnfinishedPayment.FromStartPaymentRequest(request);
        _payments[payment.Id] = payment;

        if (!_options.RequireUrlVisit)
        {
            _ = ExecutePaymentAsync(payment.Id, cancellationToken);
        }

        return new ValueTask<StartPaymentResponse>(
            StartPaymentResponse.FromPayment(payment));
    }

    public async Task<string> ConfirmPaymentAsync(
        string paymentId,
        CancellationToken cancellationToken = default)
    {
        _ = _payments.GetValueOrDefault(paymentId) ?? throw new PaymentNotFoundException();
        FinishedPaymentStatus result = await ExecutePaymentAsync(paymentId, cancellationToken);

        return result switch
        {
            FinishedPaymentStatus.Approved => "Payment confirmed",
            FinishedPaymentStatus.Declined => "Payment declined",
            _ => throw new InvalidOperationException(),
        };
    }

    private async Task<FinishedPaymentStatus> ExecutePaymentAsync(
        string paymentId,
        CancellationToken cancellationToken = default)
    {
        await Task.Delay(_options.DelayMs, cancellationToken);
        MockPayment payment =
            _payments.GetValueOrDefault(paymentId) ?? throw new PaymentNotFoundException();

        if (payment is not UnfinishedPayment processingPayment)
        {
            throw new IllegalStateException(nameof(UnfinishedPayment), payment.GetType().Name);
        }

        FinishedPaymentStatus status = _options.AllowPayments
            ? FinishedPaymentStatus.Approved
            : FinishedPaymentStatus.Declined;

        await ClosePaymentAsync(processingPayment, status, cancellationToken);
        return status;
    }

    private async Task ClosePaymentAsync(
        UnfinishedPayment processingPayment,
        FinishedPaymentStatus status,
        CancellationToken cancellationToken = default)
    {
        ClosedPayment closedPayment = processingPayment.Close(status);
        await SendWebhookAsync(
            processingPayment.ConfirmationUrl,
            processingPayment.ExternalIdentityToken,
            new FinishedPaymentWebhookRequest(
                closedPayment.Id,
                closedPayment.Status),
            cancellationToken);
    }

    private async Task SendWebhookAsync(
        Uri url,
        string identityToken,
        object body,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using HttpClient client = _clientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("X-Identity-Token", identityToken);

            using var content = new StringContent(
                JsonSerializer.Serialize(body, _jsonOptions),
                Encoding.UTF8,
                "application/json");
            HttpResponseMessage response = await client.PostAsync(url, content, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                throw new WebhookErrorException(response.StatusCode);
            }
        }
        catch (WebhookErrorException)
        {
            throw;
        }
        catch (HttpRequestException exception)
        {
            throw new WebhookErrorException(exception.StatusCode);
        }
        catch (Exception)
        {
            throw new WebhookErrorException();
        }
    }
}