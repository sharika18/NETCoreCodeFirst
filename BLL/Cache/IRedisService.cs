using System;
using System.Threading.Tasks;

namespace BLL.Cache
{
    public interface IRedisService
    {
        Task SaveAsync(string key, object value, TimeSpan? expirition = null);
        Task<T> GetAsync<T>(string key);
        Task<bool> DeleteAsync(string key);
    }
}
