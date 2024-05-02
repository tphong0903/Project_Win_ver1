using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Map
{
    public  class ProductMap : EntityTypeConfiguration<Product>
    {
        public ProductMap() 
        {
            this.HasRequired(p => p.Category)
                    .WithMany()
                    .HasForeignKey(p => p.Category_ID);

            this.HasRequired(p => p.Brand)
                    .WithMany()
                    .HasForeignKey(p => p.Brand_ID);
        }
    }
}
