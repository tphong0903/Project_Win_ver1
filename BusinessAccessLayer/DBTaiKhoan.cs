using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessAccessLayer
{
    public class DBTaiKhoan
    {
        public static string ConnStr = "";
        DBNhanVien dbnv = null;
        public DBTaiKhoan() 
        {
            dbnv = new DBNhanVien();
        }
        public bool CheckLogin(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return false;
            List<dynamic> list = dbnv.TimAllNhanVien(username);
            if (list.Any())
            {
                dynamic employee = list.First();
                string dbPassword = employee.PassWordAccount;
                int active = Int32.Parse(employee.Active);
                //ConnStr = dbnv.LayConStr(username, password);
                return active == 1 && password == dbPassword;
            }
            return false;
        }
      
    }
}
