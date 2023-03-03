using Confluent.Kafka;
using BLL.Kafka;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class FakultasConsumerService : IConsumeProcess
    {
        private readonly ILogger _logger;

        public FakultasConsumerService(ILogger logger)
        {
            _logger = logger;
        }

        public Task ConsumeAsync<TKey>(ConsumeResult<TKey, string> consumeResult, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation(consumeResult.Message.Value);
            return Task.CompletedTask;
        }
    }
}
