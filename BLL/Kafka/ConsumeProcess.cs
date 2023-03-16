using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BLL.Kafka
{
    public class ConsumeProcess : IConsumeProcess
    {
        private readonly ILogger _logger;

        public ConsumeProcess(ILogger logger)
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
