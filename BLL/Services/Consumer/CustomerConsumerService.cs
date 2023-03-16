using BLL.Interfaces;
using BLL.Kafka;
using Confluent.Kafka;
using DAL.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BLL.Services.Consumer
{
    public class CustomerConsumerService : IConsumeProcess
    {

        private readonly ILogger _logger;
        public IServiceProvider Services { get; }

        public CustomerConsumerService
            (
                ILogger logger,
                IServiceProvider services
            )
        {
            _logger = logger;
            Services = services;
        }

        public async Task ConsumeAsync<TKey>(ConsumeResult<TKey, string> consumeResult, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation(consumeResult.Message.Value);
            var orderCreatedData = JsonConvert.DeserializeObject<Sales>(consumeResult.Message.Value);
            using (var scope = Services.CreateScope())
            {
                var scopedICustomerServiceService =
                    scope.ServiceProvider
                        .GetRequiredService<ICustomerService>();

                await scopedICustomerServiceService.VerifyingCustomer(orderCreatedData);
            }
            await Task.CompletedTask;
        }
    }
}
