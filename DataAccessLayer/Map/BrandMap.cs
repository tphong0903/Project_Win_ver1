using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using DataAccessLayer.Entities;

namespace DataAccessLayer.Map
{
    public class BrandMap : EntityTypeConfiguration<Brand>
    {
        public BrandMap() 
        {
            this.HasMany(e=>e.Products).WithRequired(x => x.Brand);
        }
    }
}
