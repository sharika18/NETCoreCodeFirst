using BLL.Cache;
using BLL.DTO;
using BLL.Interfaces;
using BLL.Kafka;
using BLL.Services;
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
        private readonly ConsumerConfig _consumerConfig;
        private readonly IConfiguration _config;
        private CancellationTokenSource _cancellationTokenSource;
        private readonly ILogger _logger;
        private readonly FakultasConsumerService _fakultasConsumer;
        public IServiceProvider Services { get; }
        public ReceiveTopicVerifyCustomer(
            IConfiguration config, 
            ILogger<KafkaConsumer> logger,
            IServiceProvider services
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
            _fakultasConsumer = new FakultasConsumerService(_logger);
            Services = services;
            //_salesService = salesService;
            //_redisService = redisService;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
            var validationPartTwoComplete = _config.GetValue<string>("Topic:VerifyConsumer");

            List<Task> tasks = new List<Task>
            {
                RegisterTopic(validationPartTwoComplete,_fakultasConsumer,stoppingToken),
            };

            await Task.WhenAll(tasks.ToArray());
        }

        private Task RegisterTopic(string topic, IConsumeProcess process, CancellationToken stoppingToken)
        {
            return Task.Run(async () =>
            {
                do
                {
                    try
                    {
                        await ConsumeAsync<string>(topic, process.ConsumeAsync, false, _cancellationTokenSource.Token);
                    }
                    catch (Exception e)
                    {
                        _logger.LogCritical(e, $"Error when consuming topic \"{topic}\".");
                    }
                } while (!_cancellationTokenSource.IsCancellationRequested);
            }, stoppingToken);
        }


        private async Task ConsumeAsync<TKey>(string topic, Func<ConsumeResult<TKey, string>, CancellationToken, Task> consumeAsync, bool commitOnError, CancellationToken cancellationToken)
        {
            using (var consumer = new ConsumerBuilder<TKey, string>(_consumerConfig).Build())
            {
                consumer.Subscribe(topic);

                try
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        ConsumeResult<TKey, string> result = null;

                        try
                        {
                            result = consumer.Consume(cancellationToken);
                            using (var scope = Services.CreateScope())
                            {
                                var scopedISalesService =
                                    scope.ServiceProvider
                                        .GetRequiredService<ISalesService>();
                                var scopedIRedisService =
                                    scope.ServiceProvider
                                        .GetRequiredService<IRedisService>();

                                var verifyingCustomerDTO = Newtonsoft.Json.JsonConvert.DeserializeObject<VerifyingCustomerDTO>(result.Message.Value);

                                Sales data = await scopedIRedisService.GetAsync<Sales>($"{PrefixRedisKey.SalesKey}:{verifyingCustomerDTO.SalesId}");
                                if (data == null)
                                {
                                    data = await scopedISalesService.GetSalesByIdAsync(verifyingCustomerDTO.SalesId);
                                }

                                if (data != null)
                                {
                                    data.SalesStatus =
                                    verifyingCustomerDTO.StatusCustomer == CustomerStatus.Active ?
                                    SalesStatus.Approved : SalesStatus.Rejected;

                                    await scopedISalesService.UpdateSalesAsync(data);
                                    await scopedIRedisService.DeleteAsync($"{PrefixRedisKey.SalesKey}:{verifyingCustomerDTO.SalesId}");
                                }
                            }
                        }
                        catch (ConsumeException e)
                        {
                            _logger.LogInformation(e, e.Message);
                        }

                        if (result == null)
                        {
                            continue;
                        }

                        try
                        {
                            _logger.LogInformation($"Topic {topic} Consumed Result : {JsonConvert.SerializeObject(result)}");

                            await consumeAsync(result, cancellationToken);
                            consumer.Commit(result);
                        }
                        catch (OperationCanceledException e)
                        {
                            throw e;
                        }
                        catch (Exception e)
                        {
                            if (!commitOnError)
                            {
                                throw e;
                            }
                            consumer.Commit(result);
                        }
                    }
                }
                catch (OperationCanceledException e)
                {
                    _logger.LogWarning(e, $"Stopped consuming topic \"{topic}\".");
                }
                finally
                {
                    consumer.Close();
                }
            }
        }
    }
}
