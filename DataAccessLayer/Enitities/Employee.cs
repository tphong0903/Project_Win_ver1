using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entities
{
   
    public class Employee
    {
        [Key]
        [StringLength(5)]
        public string EmployeeID { get; set; }

        [Required]
        [StringLength(50)]
        public string NameEmployee { get; set; }

        public DateTime? Birthday { get; set; }

        [Required]
        [StringLength(3)]
        public string Gender { get; set; }

        [Required]
        [StringLength(100)]
        public string AddressEmployee { get; set; }

        [Required]
        [StringLength(12)]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(50)]
        public string RoleEmployee { get; set; }

        [Required]
        [StringLength(1)]
        public string Active { get; set; }

        [Required]
        [StringLength(10)]
        public string PassWordAccount { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public Employee() 
        {
            Orders = new HashSet<Order>(); 
        }
    }
}
