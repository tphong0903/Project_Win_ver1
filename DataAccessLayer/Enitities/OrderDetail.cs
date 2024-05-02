using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entities
{
  
    public class OrderDetail
    {
        [Key]
        [Column(Order = 1)]
        [StringLength(15)]
        [ForeignKey("Order")]
        public string Order_ID { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(15)]
        [ForeignKey("Product")]
        public string Product_ID { get; set; }

        [Required]
        public int Quantity { get; set; }

        // Navigation property to the associated order
        public virtual Order Order { get; set; }

        // Navigation property to the associated product
        public virtual Product Product { get; set; }
    }
}
