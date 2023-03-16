using DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface ISalesService
    {
        Task<List<Sales>> GetAllSalesAsync();
        Task CreateSalesAsync(Sales data);
        Task<Sales> GetSalesByIdAsync(Guid id);
        Task UpdateSalesAsync(Sales data);
        Task DeleteSalesAsync(Guid SalesId);
    }
}
