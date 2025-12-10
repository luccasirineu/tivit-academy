using Amazon.SQS;
using Amazon.SQS.Model;
using Newtonsoft.Json;

namespace tivitApi.Infra.SQS
{
    public class SQSProducer
    {
        private readonly IAmazonSQS _sqs;
        private readonly string _queueUrl;

        public SQSProducer(IConfiguration config)
        {
            _sqs = new AmazonSQSClient(
                config["AWS:AccessKey"],
                config["AWS:SecretKey"],
                Amazon.RegionEndpoint.GetBySystemName(config["AWS:Region"])
            );

            _queueUrl = config["AWS:SQSQueueUrl"];
        }

        public async Task EnviarEventoAsync<T>(T evento)
        {
            var message = new SendMessageRequest
            {
                QueueUrl = _queueUrl,
                MessageBody = JsonConvert.SerializeObject(evento)
            };

            await _sqs.SendMessageAsync(message);
        }
    }
}
