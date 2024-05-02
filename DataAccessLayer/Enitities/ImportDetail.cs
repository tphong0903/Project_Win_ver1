using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entities
{
    
    public class ImportDetail
    {
        [Key]
        [Column(Order = 1)]
        [StringLength(10)]
        [ForeignKey("Import")]
        public string Import_ID { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(15)]
        [ForeignKey("Product")]
        public string Product_ID { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public int Unitcost { get; set; }

        // Navigation property to the associated import
        public virtual Import Import { get; set; }

        // Navigation property to the associated product
        public virtual Product Product { get; set; }
    }
}
