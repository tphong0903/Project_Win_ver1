using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entities
{
    public class Brand
    {
        [Key]
        [StringLength(10)]
        public string Brand_ID { get; set; }

        [Required]
        [StringLength(50)]
        public string BrandName { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public Brand() 
        {
            Products = new HashSet<Product>();
        }
    }
}
