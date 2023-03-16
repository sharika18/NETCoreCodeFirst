using BLL.DTO;
using BLL.Interfaces;
using BLL.Kafka;
using BLL.Services;
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
    public class ReceiveTopicOrderCreated : BackgroundService
    {
        private readonly ConsumerConfig _consumerConfig;
        private readonly IConfiguration _config;
        private CancellationTokenSource _cancellationTokenSource;
        private readonly ILogger _logger;
        private readonly CustomerConsumerService _Consumer;
        private string topicVerifyConsumer = "";
        public IServiceProvider Services { get; }
        private readonly IKafkaSender _kafkaSender;
        private readonly IKafkaConsumer _kafkaConsumer;

        public ReceiveTopicOrderCreated
            (
                IConfiguration config, 
                ILogger<KafkaConsumer> logger,
                IServiceProvider services,
                IKafkaSender kafkaSender,
                IKafkaConsumer kafkaConsumer
            )
        {
            _logger = logger;
            _config = config;
            _consumerConfig = new ConsumerConfig
            {
                BootstrapServers = config.GetValue<string>("Kafka:BootstrapServers"),
                GroupId = config.GetValue<string>("Kafka:GroupId"),
                EnableAutoCommit = false,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                AllowAutoCreateTopics = true,
                IsolationLevel = IsolationLevel.ReadCommitted
            };
            Services = services;
            _kafkaSender = kafkaSender;
            topicVerifyConsumer = _config.GetValue<string>("Topic:VerifyConsumer");
            _kafkaConsumer = kafkaConsumer;

            _Consumer = new CustomerConsumerService(_logger, Services);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
            var validationPartTwoComplete = _config.GetValue<string>("Topic:OrderCreated");

            List<Task> tasks = new List<Task>
            {
              _kafkaConsumer.RegisterTopic(validationPartTwoComplete,_Consumer,stoppingToken, _cancellationTokenSource),
            };

            await Task.WhenAll(tasks.ToArray());
        }

        //private Task RegisterTopic(string topic, IConsumeProcess process, CancellationToken stoppingToken)
        //{
        //    return Task.Run(async () =>
        //    {
        //        do
        //        {
        //            try
        //            {
        //                await ConsumeAsync<string>(topic, process.ConsumeAsync, false, _cancellationTokenSource.Token);
        //            }
        //            catch (Exception e)
        //            {
        //                _logger.LogCritical(e, $"Error when consuming topic \"{topic}\".");
        //            }
        //        } while (!_cancellationTokenSource.IsCancellationRequested);
        //    }, stoppingToken);
        //}


        //private async Task ConsumeAsync<TKey>(string topic, Func<ConsumeResult<TKey, string>, CancellationToken, Task> consumeAsync, bool commitOnError, CancellationToken cancellationToken)
        //{
        //    using (var consumer = new ConsumerBuilder<TKey, string>(_consumerConfig).Build())
        //    {
        //        consumer.Subscribe(topic);

        //        try
        //        {
        //            while (!cancellationToken.IsCancellationRequested)
        //            {
        //                ConsumeResult<TKey, string> result = null;

        //                try
        //                {
        //                    result = consumer.Consume(cancellationToken);

        //                }
        //                catch (ConsumeException e)
        //                {
        //                    _logger.LogInformation(e, e.Message);
        //                }

        //                if (result == null)
        //                {
        //                    continue;
        //                }



        //                try
        //                {
        //                    _logger.LogInformation($"Topic {topic} Consumed Result : {JsonConvert.SerializeObject(result)}");

        //                    using (var scope = Services.CreateScope())
        //                    {
        //                        var scopedICustomerServiceService =
        //                            scope.ServiceProvider
        //                                .GetRequiredService<ICustomerService>();

        //                        var orderCreatedData = JsonConvert.DeserializeObject<Sales>(result.Message.Value);
        //                        await scopedICustomerServiceService.VerifyingCustomer(orderCreatedData);
        //                        await consumeAsync(result, cancellationToken);

        //                    }
        //                    consumer.Commit(result);
        //                }
        //                catch (OperationCanceledException e)
        //                {
        //                    throw e;
        //                }
        //                catch (Exception e)
        //                {
        //                    if (!commitOnError)
        //                    {
        //                        throw e;
        //                    }
        //                    consumer.Commit(result);
        //                }


        //            }
        //        }
        //        catch (OperationCanceledException e)
        //        {
        //            _logger.LogWarning(e, $"Stopped consuming topic \"{topic}\".");
        //        }
        //        finally
        //        {
        //            consumer.Close();
        //        }
        //    }
        //}
    }
}
