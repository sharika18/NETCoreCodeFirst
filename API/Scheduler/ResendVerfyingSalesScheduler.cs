
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
using DAL.Interfaces;

namespace API.Scheduler
{
    public class ResendVerfyingSalesScheduler : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _config;
        public IServiceProvider _services { get; }
        private IUnitOfWork _unitOfWork;
        public ResendVerfyingSalesScheduler(
            ILogger<ResendVerfyingSalesScheduler> logger,
            IConfiguration config,
            IServiceProvider services,
            IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _config = config;
            _services = services;
            _unitOfWork = unitOfWork;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            bool scheduler = true;

            var cronJob = _config.GetValue<string>("Cron:resendVerifyingSales");
            if (cronJob == null)
            {
                scheduler = false;
            }

            if (scheduler)
            {
                await Task.Delay(5000, stoppingToken);

                _logger.LogInformation($"Resend verifying sales background proccess started");
                while (!stoppingToken.IsCancellationRequested)
                {
                    var CronDaemon = new CronDaemon();

                    CronDaemon.Start();
                    CronDaemon.Add(cronJob, () =>
                    {
                        _logger.LogInformation($"Resend verifying sales start");

                        using (var scope = _services.CreateScope())
                        {
                            
                                var scopedICustomerServiceService =
                                scope.ServiceProvider
                                    .GetRequiredService<ISalesService>();

                            scopedICustomerServiceService.ResendVerifyingSales();
                        }
                    });
                    while (true) Thread.Sleep(6000);
                }
             }
        }
    }
}
