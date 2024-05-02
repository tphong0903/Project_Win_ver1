using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entities
{
    public class Category
    {
        [Key]
        [StringLength(10)]
        public string Category_ID { get; set; }

        [Required]
        [StringLength(50)]
        public string CategoryName { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public Category()
        {
            Products = new HashSet<Product>();
        }
    }
}
