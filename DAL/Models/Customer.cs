using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    public class Customer
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid CustomerId { get; set; }

        [StringLength(100)]
        public string FirstName { get; set; }
        [StringLength(100)]
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        [StringLength(1)]
        public string MaritalStatus { get; set; }
        [StringLength(1)]
        public string Gender { get; set; }
        public string AddressLine { get; set; }
        public DateTime? DateFirstPurchase { get; set; }

        public bool CustomerIsActive { get; set; }

        public Customer()
        {
            CustomerIsActive = false;
        }
    }
}
