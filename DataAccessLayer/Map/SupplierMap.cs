using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Map
{
    public class SupplierMap : EntityTypeConfiguration<Supplier>
    {
        public SupplierMap()
        {
            this.HasMany(e => e.Imports).WithRequired(x => x.Supplier);
        }
    }
}
