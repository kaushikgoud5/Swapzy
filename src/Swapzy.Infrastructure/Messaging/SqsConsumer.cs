using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Swapzy.Application.Interfaces;
using System.Text.Json;

namespace Swapzy.Infrastructure.Messaging
{
    public class SqsConsumer : BackgroundService
    {
        private readonly IAmazonSQS _sqs;
        private readonly string _queueUrl;
        private readonly ILogger<SqsConsumer> _logger;
        private readonly IServiceProvider _serviceProvider;

        public SqsConsumer(IAmazonSQS sqs, IConfiguration configuration, ILogger<SqsConsumer> logger, IServiceProvider serviceProvider)
        {
            _sqs = sqs;
            _queueUrl = configuration["AWS:SqsQueueUrl"]!;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var response = await _sqs.ReceiveMessageAsync(new ReceiveMessageRequest
                    {
                        QueueUrl = _queueUrl,
                        MaxNumberOfMessages = 10,
                        WaitTimeSeconds = 20,
                        MessageAttributeNames = new List<string> { "All" }
                    }, stoppingToken);

                    foreach (var msg in response.Messages)
                    {
                        try
                        {
                            await ProcessMessageAsync(msg, stoppingToken);
                            await _sqs.DeleteMessageAsync(_queueUrl, msg.ReceiptHandle, stoppingToken);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to process SQS message {MessageId}", msg.MessageId);
                        }
                    }
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "SQS polling error");
                    await Task.Delay(5000, stoppingToken);
                }
            }
        }

        private async Task ProcessMessageAsync(Message message, CancellationToken ct)
        {
            var (eventType, body) = ExtractEventData(message);

            if (eventType == null)
            {
                _logger.LogWarning("No EventType in message {MessageId}", message.MessageId);
                return;
            }

            using var scope = _serviceProvider.CreateScope();
            var handlers = scope.ServiceProvider.GetServices<IEventHandler>();
            var handler = handlers.FirstOrDefault(h => h.EventType == eventType);

            if (handler == null)
            {
                _logger.LogWarning("No handler for EventType: {EventType}", eventType);
                return;
            }

            await handler.HandleAsync(body, ct);
        }

        private static (string? eventType, string body) ExtractEventData(Message message)
        {
            // Try message attributes first
            if (message.MessageAttributes.TryGetValue("EventType", out var attr))
                return (attr.StringValue, message.Body);

            // SNS wraps message in an envelope — try to unwrap
            try
            {
                var doc = JsonDocument.Parse(message.Body);

                // SNS envelope has "Message" and "MessageAttributes" fields
                if (doc.RootElement.TryGetProperty("Message", out var msgProp))
                {
                    var innerBody = msgProp.GetString() ?? message.Body;

                    // Try to get EventType from SNS MessageAttributes
                    if (doc.RootElement.TryGetProperty("MessageAttributes", out var attrs) &&
                        attrs.TryGetProperty("EventType", out var etAttr) &&
                        etAttr.TryGetProperty("Value", out var etVal))
                    {
                        return (etVal.GetString(), innerBody);
                    }

                    // Fallback: parse EventType from inner message body
                    var innerDoc = JsonDocument.Parse(innerBody);
                    if (innerDoc.RootElement.TryGetProperty("EventType", out var innerEt))
                        return (innerEt.GetString(), innerBody);
                }

                // Not an SNS envelope, try direct EventType
                if (doc.RootElement.TryGetProperty("EventType", out var directEt))
                    return (directEt.GetString(), message.Body);
            }
            catch { }

            return (null, message.Body);
        }
    }
}
