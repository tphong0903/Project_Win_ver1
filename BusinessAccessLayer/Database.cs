using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
//
using DataAccessLayer;
using System.Data.Entity;
using DataAccessLayer.Entities;
using DataAccessLayer.Map;

namespace BusinessAccessLayer
{
    public class Database
    {
        public Database()
        {
            using (var context = new QLCuaHang())
            {
                System.Data.Entity.Database.SetInitializer(new Initializer());
            }
        }
    }
}
