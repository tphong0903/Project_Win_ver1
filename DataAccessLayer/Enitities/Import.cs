using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entities
{
    
    public class Import
    {
        [Key]
        [StringLength(10)]
        public string Import_ID { get; set; }

        [Required]
        [ForeignKey("Supplier")]
        [StringLength(10)]
        public string Supplier_ID { get; set; }

        [Required]
        public DateTime ImportDay { get; set; }

        [Required]
        public int Total { get; set; }

        // Navigation property to the associated supplier
        public virtual Supplier Supplier { get; set; }

        public virtual ICollection<ImportDetail> ImportDetails { get; set; }
        public Import()
        {
            ImportDetails = new HashSet<ImportDetail>();
        }
    }
}
