using BLL.Kafka;
using BLL.Services;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BLL.Background
{
    public class SalesReceiveKafka : BackgroundService
    {
        private CancellationTokenSource _cancellationTokenSource;
        private readonly IConfiguration _config;
        private readonly SalesConsumerService _salesConsumer;
        private readonly IKafkaConsumer _kafkaConsumer;
        private readonly ILogger _logger;

        public SalesReceiveKafka(IConfiguration config, ILogger<SalesReceiveKafka> logger, IKafkaConsumer kafkaConsumer)
        {
            _logger = logger;
            _config = config;
            _salesConsumer = new SalesConsumerService(_logger);
            _kafkaConsumer = kafkaConsumer;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
            var validationPartTwoComplete = _config.GetValue<string>("Topic:Sales");

            List<Task> tasks = new List<Task>
            {
               _kafkaConsumer.RegisterTopic(validationPartTwoComplete,_salesConsumer,stoppingToken, _cancellationTokenSource),
            };

            await Task.WhenAll(tasks.ToArray());
        }
    }
}
