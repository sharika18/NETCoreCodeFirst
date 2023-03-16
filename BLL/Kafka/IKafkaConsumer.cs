using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BLL.Kafka
{
    public interface IKafkaConsumer
    {
        Task RegisterTopic(string topic, IConsumeProcess process, CancellationToken stoppingToken, CancellationTokenSource cancellationTokenSource);
    }
}
