using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Kafka
{
    public interface IKafkaSender
    {
        Task SendAsync(string topic, object message);
    }
}
