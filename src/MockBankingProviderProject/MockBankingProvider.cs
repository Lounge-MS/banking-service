using BankingServiceProject.MockBankingProviderProject.Domain;
using BankingServiceProject.MockBankingProviderProject.Exceptions;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;

namespace BankingServiceProject.MockBankingProviderProject;

public class MockBankingProvider
{
    private readonly ConcurrentDictionary<string, MockPayment> _payments = new();
    private readonly ConcurrentDictionary<string, UnfinishedPayment> _processingPayments = new();
    private readonly ConcurrentDictionary<string, RollbackPayment> _rollbackPayments = new();
    private readonly MockBankingOptions _options;
    private readonly IHttpClientFactory _clientFactory;

    public MockBankingProvider(
        IOptions<MockBankingOptions> options,
        IHttpClientFactory httpClientFactory)
    {
        _options = options.Value;
        _clientFactory = httpClientFactory;
    }

    public ValueTask<StartPaymentResponse> StartPaymentAsync(
        StartPaymentRequest request,
        CancellationToken cancellationToken = default)
    {
        var payment = MockPayment.FromStartPaymentRequest(request);
        var processingPayment = new UnfinishedPayment(
            payment,
            new Uri(request.ConfirmationUrl),
            request.IdentityToken);

        _payments[payment.Id] = payment;
        _processingPayments[payment.Id] = processingPayment;

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

        FinishedPaymentStatus status = await ExecutePaymentAsync(paymentId, cancellationToken);
        return status switch
        {
            FinishedPaymentStatus.Approved => "Payment confirmed",
            FinishedPaymentStatus.Declined => "Payment declined",
            _ => throw new InvalidOperationException(),
        };
    }

    public void StartRollback(
        StartRollbackRequest request,
        string externalIdentityToken,
        CancellationToken cancellationToken = default)
    {
        MockPayment payment =
            _payments.GetValueOrDefault(request.PaymentId) ?? throw new PaymentNotFoundException();
        payment.ValidateToken(externalIdentityToken);

        payment.ValidateState(MockPaymentStatus.Approved);

        var rollbackPayment = new RollbackPayment(
            payment,
            new Uri(request.ConfirmationUrl),
            request.IdentityToken);
        payment.Status = MockPaymentStatus.OnRollback;

        _rollbackPayments[payment.Id] = rollbackPayment;

        _ = ExecuteRollbackAsync(
            payment.Id,
            cancellationToken);
    }

    private async Task ExecuteRollbackAsync(
        string paymentId,
        CancellationToken cancellationToken = default)
    {
        await Task.Delay(_options.DelayMs, cancellationToken);
        _rollbackPayments.TryRemove(paymentId, out RollbackPayment? rollbackPayment);

        if (rollbackPayment == null || rollbackPayment.Payment.Status != MockPaymentStatus.Approved)
        {
            throw new IllegalStateException();
        }

        if (_options.AllowRollbacks)
        {
            await AcceptRollbackAsync(
                rollbackPayment,
                cancellationToken);
        }
        else
        {
            await DeclineRollbackAsync(
                rollbackPayment,
                cancellationToken);
        }
    }

    private async Task AcceptRollbackAsync(
        RollbackPayment rollbackPayment,
        CancellationToken cancellationToken = default)
    {
        rollbackPayment.Payment.Status = MockPaymentStatus.Rollback;
        await SendWebhookAsync(
            rollbackPayment.ConfirmationUrl,
            rollbackPayment.ExternalIdentityToken,
            new FinishedRollbackWebhookRequest(
                rollbackPayment.Payment.Id,
                FinishedRollbackStatus.Approved),
            cancellationToken);
    }

    private async Task DeclineRollbackAsync(
        RollbackPayment rollbackPayment,
        CancellationToken cancellationToken = default)
    {
        await SendWebhookAsync(
            rollbackPayment.ConfirmationUrl,
            rollbackPayment.ExternalIdentityToken,
            new FinishedRollbackWebhookRequest(
                rollbackPayment.Payment.Id,
                FinishedRollbackStatus.Declined),
            cancellationToken);
    }

    private async Task<FinishedPaymentStatus> ExecutePaymentAsync(
        string paymentId,
        CancellationToken cancellationToken = default)
    {
        await Task.Delay(_options.DelayMs, cancellationToken);
        _processingPayments.TryRemove(paymentId, out UnfinishedPayment? processingPayment);

        if (processingPayment == null || processingPayment.Payment.Status != MockPaymentStatus.Created)
        {
            throw new IllegalStateException();
        }

        if (_options.AllowPayment)
        {
            await AcceptPaymentAsync(
                processingPayment,
                cancellationToken);
            return FinishedPaymentStatus.Approved;
        }
        else
        {
            await DeclinePaymentAsync(
                processingPayment,
                cancellationToken);
            return FinishedPaymentStatus.Declined;
        }
    }

    private async Task AcceptPaymentAsync(
        UnfinishedPayment processingPayment,
        CancellationToken cancellationToken = default)
    {
        processingPayment.Payment.Status = MockPaymentStatus.Approved;
        await SendWebhookAsync(
            processingPayment.ConfirmationUrl,
            processingPayment.ExternalIdentityToken,
            new FinishedPaymentWebhookRequest(
                processingPayment.Payment.Id,
                FinishedPaymentStatus.Approved),
            cancellationToken);
    }

    private async Task DeclinePaymentAsync(
        UnfinishedPayment processingPayment,
        CancellationToken cancellationToken = default)
    {
        processingPayment.Payment.Status = MockPaymentStatus.Declined;
        await SendWebhookAsync(
            processingPayment.ConfirmationUrl,
            processingPayment.ExternalIdentityToken,
            new FinishedPaymentWebhookRequest(
                processingPayment.Payment.Id,
                FinishedPaymentStatus.Declined),
            cancellationToken);
    }

    private async Task SendWebhookAsync(
        Uri url,
        string identityToken,
        object body,
        CancellationToken cancellationToken = default)
    {
        using HttpClient client = _clientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Identity-Token", identityToken);

        using var content = new StringContent(
            JsonSerializer.Serialize(body),
            Encoding.UTF8,
            "application/json");
        await client.PostAsync(url, content, cancellationToken);
    }
}