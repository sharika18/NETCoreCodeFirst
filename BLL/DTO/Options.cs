using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.DTO
{
    public static class CustomerStatus
    {
        public const string Active = "Active";
        public const string InActive = "InActive";
        public const string NotFound = "NotFound";
    }

    public static class SalesStatus
    {
        public const string Verifying = "Verifying";
        public const string Approved = "Approved";
        public const string Rejected = "Rejected";
    }
}
