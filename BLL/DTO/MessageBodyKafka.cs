using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.DTO
{
    public class VerifyingCustomerDTO
    {
        public Guid SalesId { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerStatus { get; set; }
    }
}
