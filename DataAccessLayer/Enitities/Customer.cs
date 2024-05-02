using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entities
{
    public class Customer
    {
        [Key]
        [StringLength(12)]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(50)]
        public string NameCustomer { get; set; }

        public DateTime? Birthday { get; set; }

        [Required]
        [StringLength(3)]
        public string Gender { get; set; }

        public int? Point { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public Customer() 
        {
            Orders = new HashSet<Order>();
        }
    }
}
