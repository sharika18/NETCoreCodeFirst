
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cron;
using BLL.Interfaces;
using Quartz;

namespace BLL.Scheduler
{
    public class ResendVerfyingSalesScheduler : IJob
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _config;
        public IServiceProvider _services { get; }
        public ResendVerfyingSalesScheduler(
            ILogger<ResendVerfyingSalesScheduler> logger,
            IConfiguration config,
            IServiceProvider services)
        {
            _logger = logger;
            _config = config;
            _services = services;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation($"Current Date : {DateTime.UtcNow}");
            bool scheduler = true;

            var cronJob = _config.GetValue<string>("Cron:resendVerifyingSales");
            if (cronJob == null)
            {
                scheduler = false;
            }

            if (scheduler)
            {
                _logger.LogInformation($"Resend verifying sales start");

                using (var scope = _services.CreateScope())
                {
                    var scopedICustomerServiceService =
                        scope.ServiceProvider
                            .GetRequiredService<ISalesService>();

                    await scopedICustomerServiceService.ResendVerifyingSales();
                }
            }
        }
    }
}
