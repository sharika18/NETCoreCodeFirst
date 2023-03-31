using BLL.Kafka;
using DAL.Models;
using DAL.Repositories;
using External;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.Reflection;
using BLL.Services;
using BLL.Background;
using BLL.Cache;
using BLL.Interfaces;
using DAL.Interfaces;
using API.Hubs;
using BLL.Scheduler;
using BLL;

namespace API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddDbContext<Context>
            (
                options =>
                options
                .UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))
            );

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddScoped<IRedisService, RedisService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ISalesService, SalesService>();


            services.AddSingleton<IKafkaSender, KafkaSender>();
            services.AddSingleton<IKafkaConsumer, KafkaConsumer>();

            services.AddSingleton<IHostedService, ConsumerService>();
            //services.AddSingleton<IHostedService, SalesReceiveKafka>();
            //services.AddSingleton<IHostedService, ReceiveTopicOrderCreated>();
            services.AddHostedService<ReceiveTopicOrderCreated>();
            services.AddHostedService<ReceiveTopicVerifyCustomer>();
            //services.AddHostedService<ResendVerfyingSalesScheduler>();


            services.AddApplicationInsightsTelemetry();

            services.AddTransient<ResendVerfyingSalesScheduler>();
            services.AddTransient<QuartzJobFactory>();

            services.AddSingleton<ISchedulerService, SchedulerService>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "The API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });



            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "NET Core Code First");
            });

            //ini buat inisialisasi shedulernya dan trigger start nya
            var schedulerService = app.ApplicationServices.GetRequiredService<ISchedulerService>();
            schedulerService.Initialize();
            schedulerService.Start();


        }
    }
}
