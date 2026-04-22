using Amazon.SQS;
using Amazon.SQS.Model;
using Newtonsoft.Json;

namespace tivitApi.Infra.SQS
{
    public interface ISQSProducer
    {
        Task EnviarEventoAsync<T>(T evento); 
    }
    
    public class SQSProducer : ISQSProducer
    {
        private readonly IAmazonSQS _sqs;
        private readonly string _queueUrl;

         public SQSProducer(IAmazonSQS sqs, IConfiguration config)
        {
            _sqs = sqs;
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
