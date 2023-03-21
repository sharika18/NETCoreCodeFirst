using BLL.Cache;
using BLL.DTO;
using BLL.Interfaces;
using BLL.Kafka;
using BLL.Services.Consumer;
using Confluent.Kafka;
using DAL.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BLL.Background
{
    public class ReceiveTopicVerifyCustomer : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _config;
        public IServiceProvider _services { get; }
        private CancellationTokenSource _cancellationTokenSource;
        private readonly IKafkaConsumer _kafkaConsumer;
        private string topicToConsume = "";
        private readonly SalesConsumerService _Consumer;
        public ReceiveTopicVerifyCustomer
            (
                IConfiguration config, 
                ILogger<KafkaConsumer> logger,
                IServiceProvider services,
                IKafkaConsumer kafkaConsumer
            )
        {
            _logger = logger;
            _config = config;
            _services = services;
            _kafkaConsumer = kafkaConsumer;
            topicToConsume = _config.GetValue<string>("Topic:VerifyConsumer");
            _Consumer = new SalesConsumerService(_logger, _services);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
            List<Task> tasks = new List<Task>
            {
              _kafkaConsumer.RegisterTopic(topicToConsume,_Consumer,stoppingToken, _cancellationTokenSource),
            };

            await Task.WhenAll(tasks.ToArray());
        }
    }
}
