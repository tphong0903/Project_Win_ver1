using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entities
{
   
    public class PictureProduct
    {
        [Key]
        public int Picture_ID { get; set; }

        [StringLength(100)]
        public string Picture_Name { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public PictureProduct()
        {
            Products = new HashSet<Product>();
        }

    }
}
