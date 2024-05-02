using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Map
{
    public class PictureProductMap : EntityTypeConfiguration<PictureProduct>
    {
        public PictureProductMap()
        {
            this.HasMany(e => e.Products).WithRequired(x => x.PictureProduct);
        }
    }
}
