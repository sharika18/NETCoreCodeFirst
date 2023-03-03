using Confluent.Kafka;
using System.Threading;
using System.Threading.Tasks;

namespace BLL.Kafka
{
    public interface IConsumeProcess
    {
        Task ConsumeAsync<TKey>(ConsumeResult<TKey, string> consumeResult, CancellationToken cancellationToken = default);
    }
}
