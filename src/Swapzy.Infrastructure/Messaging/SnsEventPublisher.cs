using Amazon.Runtime.Internal.Util;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Swapzy.Application.Interfaces;
using Swapzy.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Swapzy.Infrastructure.Messaging
{
    public class SnsEventPublisher : IEventPublisher
    {
        private readonly IAmazonSimpleNotificationService _sns;
        private readonly string _topicArn;
        private readonly ILogger<SnsEventPublisher> _logger;
        public SnsEventPublisher(IAmazonSimpleNotificationService sns, IConfiguration configuration, ILogger<SnsEventPublisher> logger)
        {
            _sns = sns;
            _topicArn = configuration["AWS:SnsTopicArn"] ?? "";
            _logger = logger;
        }
        public async Task PublishAsync<T>(T domainEvent, CancellationToken ct = default) where T : DomainEvent
        {
            var message = JsonSerializer.Serialize(domainEvent);
            var req = new PublishRequest
            {
                TopicArn = _topicArn,
                Message = message,
                MessageAttributes = new Dictionary<string, MessageAttributeValue>
                {
                    { "EventType", new MessageAttributeValue { DataType = "String", StringValue = domainEvent.EventType } }
                }
            };

            await _sns.PublishAsync(req, ct);
        }
    }
}
