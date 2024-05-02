using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entities
{
   
    public class Product
    {
        [Key]
        [StringLength(15)]
        public string Product_ID { get; set; }

        [Required]
        [StringLength(100)]
        public string ProductName { get; set; }

        [Required]
        public int UnitPrice { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [StringLength(10)]
        [ForeignKey("Brand")]
        public string Brand_ID { get; set; }

        [Required]
        [StringLength(10)]
        [ForeignKey("Category")]
        public string Category_ID { get; set; }

        [ForeignKey("PictureProduct")]
        public int Picture_ID { get; set; }

        // Navigation property to the associated brand
        public virtual Brand Brand { get; set; }

        // Navigation property to the associated category
        public virtual Category Category { get; set; }

        // Navigation property to the associated picture
        public virtual PictureProduct PictureProduct { get; set; }
    }

}
