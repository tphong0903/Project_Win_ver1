using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using DataAccessLayer.Entities;

namespace DataAccessLayer.Map
{
    public class CustomerMap : EntityTypeConfiguration<Customer>
    {
        public CustomerMap() 
        {
            this.HasMany(e=>e.Orders).WithRequired(e=>e.Customer);
        }
    }
}
