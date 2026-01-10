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

    public ValueTask StartPayment(
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
            _ = ExecutePayment(payment.Id, cancellationToken);
        }

        return ValueTask.CompletedTask;
    }

    public async Task<string> ConfirmPayment(
        string paymentId,
        CancellationToken cancellationToken = default)
    {
        _ = _payments.GetValueOrDefault(paymentId) ?? throw new PaymentNotFoundException();

        FinishedPaymentStatus status = await ExecutePayment(paymentId, cancellationToken);
        return status switch
        {
            FinishedPaymentStatus.Approved => "Payment confirmed",
            FinishedPaymentStatus.Declined => "Payment declined",
            _ => throw new InvalidOperationException(),
        };
    }

    public void StartRollback(
        StartRollbackRequest request,
        CancellationToken cancellationToken = default)
    {
        MockPayment payment =
            _payments.GetValueOrDefault(request.PaymentId) ?? throw new PaymentNotFoundException();

        var rollbackPayment = new RollbackPayment(
            payment,
            new Uri(request.ConfirmationUrl),
            request.IdentityToken);

        _ = ExecuteRollback(
            payment.Id,
            cancellationToken);
    }

    private async Task ExecuteRollback(
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
            await AcceptRollback(
                rollbackPayment,
                cancellationToken);
        }
        else
        {
            await DeclineRollback(
                rollbackPayment,
                cancellationToken);
        }
    }

    private async Task AcceptRollback(
        RollbackPayment rollbackPayment,
        CancellationToken cancellationToken = default)
    {
        rollbackPayment.Payment.Status = MockPaymentStatus.Rollback;
        await SendWebhook(
            rollbackPayment.ConfirmationUrl,
            rollbackPayment.IdentityToken,
            new FinishedRollbackWebhookRequest(
                rollbackPayment.Payment.Id,
                FinishedRollbackStatus.Approved),
            cancellationToken);
    }

    private async Task DeclineRollback(
        RollbackPayment rollbackPayment,
        CancellationToken cancellationToken = default)
    {
        await SendWebhook(
            rollbackPayment.ConfirmationUrl,
            rollbackPayment.IdentityToken,
            new FinishedRollbackWebhookRequest(
                rollbackPayment.Payment.Id,
                FinishedRollbackStatus.Declined),
            cancellationToken);
    }

    private async Task<FinishedPaymentStatus> ExecutePayment(
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
            await AcceptPayment(
                processingPayment,
                cancellationToken);
            return FinishedPaymentStatus.Approved;
        }
        else
        {
            await DeclinePayment(
                processingPayment,
                cancellationToken);
            return FinishedPaymentStatus.Declined;
        }
    }

    private async Task AcceptPayment(
        UnfinishedPayment processingPayment,
        CancellationToken cancellationToken = default)
    {
        processingPayment.Payment.Status = MockPaymentStatus.Approved;
        await SendWebhook(
            processingPayment.ConfirmationUrl,
            processingPayment.IdentityToken,
            new FinishedPaymentWebhookRequest(
                processingPayment.Payment.Id,
                FinishedPaymentStatus.Approved),
            cancellationToken);
    }

    private async Task DeclinePayment(
        UnfinishedPayment processingPayment,
        CancellationToken cancellationToken = default)
    {
        processingPayment.Payment.Status = MockPaymentStatus.Declined;
        await SendWebhook(
            processingPayment.ConfirmationUrl,
            processingPayment.IdentityToken,
            new FinishedPaymentWebhookRequest(
                processingPayment.Payment.Id,
                FinishedPaymentStatus.Declined),
            cancellationToken);
    }

    private async Task SendWebhook(
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