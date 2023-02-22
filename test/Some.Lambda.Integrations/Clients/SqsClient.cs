using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Some.Lambda.Integrations.Clients
{
    public class SqsClient
    {
        private readonly AmazonSQSClient _sqsClient;
        private readonly string _queueUrl;

        public SqsClient()
        {
            var env = Environment.GetEnvironmentVariable("Application__AppEnvironment") ?? "test";

            if (string.IsNullOrWhiteSpace(env))
                throw new Exception("couldn't determine environment, make sure that env variable 'Application__AppEnvironment' is set");

            var builder = new ConfigurationBuilder()
                .AddJsonFile($"config/appsettings.{env}.json", false)
                .AddEnvironmentVariables();

            var config = builder.Build();
            var queueConfig = config.GetSection(nameof(QueueConfig)).Get<QueueConfig>();

            if (queueConfig == null)
                throw new ArgumentNullException(nameof(QueueConfig), "Queue configuration can not be null");

            _sqsClient = new AmazonSQSClient(RegionEndpoint.GetBySystemName(queueConfig.Region));
            _queueUrl = queueConfig.QueueUrl;
        }

        public async Task<SendMessageResponse> PublishMessageAsync(string messageBody, string attributeValue)
        {
            return await _sqsClient.SendMessageAsync(new SendMessageRequest
            {
                QueueUrl = _queueUrl,
                MessageBody = messageBody,
                MessageAttributes = !string.IsNullOrEmpty(attributeValue) ? CreateMessageAttribute(attributeValue) : new Dictionary<string, MessageAttributeValue>()
            });
        }

        private static Dictionary<string, MessageAttributeValue> CreateMessageAttribute(string message)
        {
            return new Dictionary<string, MessageAttributeValue>
            {
                {"Type", new MessageAttributeValue {StringValue = message, DataType = "String"}}
            };
        }
    }
}
