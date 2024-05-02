using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Map
{
    public class DiscountMap : EntityTypeConfiguration<Discount>
    {
        public DiscountMap()
        {
            this.HasMany(e => e.Orders).WithOptional(x => x.Discount);
        }
    }
}
