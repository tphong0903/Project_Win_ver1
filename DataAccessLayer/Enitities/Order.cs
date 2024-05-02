using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entities
{
    
    public class Order
    {
        [Key]
        [StringLength(15)]
        public string Order_ID { get; set; }

        [Required]
        [StringLength(12)]
        [ForeignKey("Customer")]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(5)]
        [ForeignKey("Employee")]
        public string EmployeeID { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        public int Total { get; set; }

        
        [ForeignKey("Discount")]
        [StringLength(10)]
        public string DiscountCode { get; set; }

        // Navigation property to the associated customer
        public virtual Customer Customer { get; set; }

        // Navigation property to the associated employee
        public virtual Employee Employee { get; set; }

        // Navigation property to the associated discount
        public virtual Discount Discount { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }
    }
}
