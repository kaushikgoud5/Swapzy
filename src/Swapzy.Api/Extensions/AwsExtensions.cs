using Amazon.Extensions.NETCore.Setup;
using Amazon.S3;
using Amazon.SimpleNotificationService;
using Amazon.SQS;
using Swapzy.Application.Interfaces;
using Swapzy.Infrastructure.Messaging;
using Swapzy.Infrastructure.Messaging.Handlers;
using Swapzy.Infrastructure.Storage;

namespace Swapzy.Api.Extensions;

public static class AwsExtensions
{
    public static IServiceCollection AddSwapzyAws(this IServiceCollection services, IConfiguration configuration)
    {
        var awsOptions = configuration.GetAWSOptions();
        var serviceUrl = configuration["AWS:ServiceURL"];

        if (!string.IsNullOrEmpty(serviceUrl))
        {
            var region = awsOptions.Region?.SystemName ?? "ap-south-1";
            services.AddSingleton<IAmazonSimpleNotificationService>(
                new AmazonSimpleNotificationServiceClient(new AmazonSimpleNotificationServiceConfig { ServiceURL = serviceUrl, AuthenticationRegion = region }));
            services.AddSingleton<IAmazonSQS>(
                new AmazonSQSClient(new AmazonSQSConfig { ServiceURL = serviceUrl, AuthenticationRegion = region }));
            services.AddSingleton<IAmazonS3>(
                new AmazonS3Client(new AmazonS3Config { ServiceURL = serviceUrl, ForcePathStyle = true, AuthenticationRegion = region }));
        }
        else
        {
            services.AddAWSService<IAmazonSimpleNotificationService>(awsOptions);
            services.AddAWSService<IAmazonSQS>(awsOptions);
            services.AddAWSService<IAmazonS3>(awsOptions);
        }

        services.AddScoped<IEventPublisher, SnsEventPublisher>();
        services.AddScoped<IEventHandler, ProductCreatedHandler>();
        services.AddScoped<IEventHandler, InterestCreatedHandler>();
        services.AddScoped<IStorageService, S3StorageService>();

        if (configuration.GetValue<bool>("AWS:EnableMessaging"))
            services.AddHostedService<SqsConsumer>();

        return services;
    }
}
