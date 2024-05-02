using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Map
{
    public class ImportMap : EntityTypeConfiguration<Import>
    {
        public ImportMap()
        {
            this.HasMany(e => e.ImportDetails).WithRequired(x => x.Import);
        }
    }
}
