using DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface ICustomerService
    {
        Task<List<Customer>> GetAllCustomerAsync();
        Task CreateCustomerAsync(Customer data);
        Task<Customer> GetCustomerByIdAsync(Guid id);
        Task UpdateCustomerAsync(Customer data);
        Task DeleteCustomerAsync(Guid CustomerId);
        Task VerifyingCustomer(Sales data);
    }
}
