using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using Swapzy.Application.Interfaces;

namespace Swapzy.Infrastructure.Storage
{
    public class S3StorageService : IStorageService
    {
        private readonly IAmazonS3 _s3;
        private readonly string _bucketName;

        public S3StorageService(IAmazonS3 s3, IConfiguration configuration)
        {
            _s3 = s3;
            _bucketName = configuration["AWS:S3BucketName"]!;
        }

        public Task<string> GenerateUploadUrlAsync(string key, string contentType, int expirationMinutes = 15)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _bucketName,
                Key = key,
                Verb = HttpVerb.PUT,
                Expires = DateTime.UtcNow.AddMinutes(expirationMinutes),
                ContentType = contentType
            };

            return Task.FromResult(_s3.GetPreSignedURL(request));
        }

        public Task<string> GenerateReadUrlAsync(string key, int expirationMinutes = 60)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _bucketName,
                Key = key,
                Verb = HttpVerb.GET,
                Expires = DateTime.UtcNow.AddMinutes(expirationMinutes)
            };

            return Task.FromResult(_s3.GetPreSignedURL(request));
        }

        public async Task DeleteAsync(string key)
        {
            await _s3.DeleteObjectAsync(_bucketName, key);
        }
    }
}
