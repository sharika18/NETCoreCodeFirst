using BLL.DTO;
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
        Task<Sales> GetSalesByIdAsync(Guid id);
        Task CreateSalesAsync(Sales data);
        Task UpdateSalesAsync(Sales data);
        Task ApproveRejectSales(VerifyingCustomerDTO verifyingCustomerdata);

        //Territories

        Task<List<Territories>> GetAllTerritoriesAsync();
    }
}
