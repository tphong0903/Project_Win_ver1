using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entities
{
    public class Discount
    {
        [Key]
        [StringLength(10)]
        public string DiscountCode { get; set; }

        [Required]
        public int PercentageDiscount { get; set; }

        [Required]
        public DateTime StartDay { get; set; }

        [Required]
        public DateTime EndDay { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public Discount()
        {
            Orders = new HashSet<Order>();
        }
    }
}
