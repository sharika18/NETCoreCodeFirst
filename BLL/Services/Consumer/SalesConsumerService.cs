using BLL.DTO;
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
    public class SalesConsumerService : IConsumeProcess
    {
        private readonly ILogger _logger;
        public IServiceProvider Services { get; }

        public SalesConsumerService
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
            _logger.LogInformation($"Topic Consumed : {consumeResult.Message.Value}");
            var verifyingCustomerDTO = JsonConvert.DeserializeObject<VerifyingCustomerDTO>(consumeResult.Message.Value);
            using (var scope = Services.CreateScope())
            {
                var scopedISalesServiceService =
                    scope.ServiceProvider
                        .GetRequiredService<ISalesService>();
                await scopedISalesServiceService.ApproveRejectSales(verifyingCustomerDTO);

            }
            await Task.CompletedTask;
        }
    }
}
