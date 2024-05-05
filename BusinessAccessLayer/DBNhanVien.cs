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

namespace BusinessAccessLayer // Declaring the BusinessAccessLayer namespace
{
    public class DBNhanVien // Declaring the DBNhanVien class
    {

        public DBNhanVien()
        {
         
        }

        // Method to retrieve active employees
        public List<dynamic> LayNhanVien()
        {
            using(var context = new QLCuaHang())
            {
                var query = from p in context.Employees.Include(e=>e.Orders)
                            where p.Active=="1"
                            select new 
                            {
                                p.EmployeeID,
                                p.NameEmployee,
                                p.Birthday,
                                p.Gender,
                                p.AddressEmployee,
                                p.PhoneNumber,
                                p.RoleEmployee,
                                Tong = p.Orders.Any() ? p.Orders.Sum(o => (int?)o.Total) : 0,
                               
                            };
                return query.ToList<dynamic>();
            }
           
        }

        // Method to retrieve all employees
        public List<dynamic> LayALLNhanVien()
        {
            using (var context = new QLCuaHang())
            {
                var query = from p in context.Employees.Include(e => e.Orders)
                            select new
                            {
                                p.EmployeeID,
                                p.NameEmployee,
                                p.Birthday,
                                p.Gender,
                                p.AddressEmployee,
                                p.PhoneNumber,
                                p.RoleEmployee,
                                p.Active,
                                p.PassWordAccount,
                                Tong = p.Orders.Any() ? p.Orders.Sum(o => (int?)o.Total) : 0,

                            };
                return query.ToList<dynamic>();
            }
        }

        public List<dynamic> TimNhanVien(string ID, string name)
        {
            using (var context = new QLCuaHang())
            {
                var query = from p in context.Employees.Include(e => e.Orders)
                            where p.EmployeeID.Contains(ID) && p.NameEmployee.Contains(name)
                            select new
                            {
                                p.EmployeeID,
                                p.NameEmployee,
                                p.Birthday,
                                p.Gender,
                                p.RoleEmployee,
                                Tong = p.Orders.Any() ? p.Orders.Sum(o => (int?)o.Total) : 0,

                            };
                return query.ToList<dynamic>();
            }
        }

        // Method to search for all employees by ID
        public List<dynamic> TimAllNhanVien(string ID)
        {
            using (var context = new QLCuaHang())
            {
                var query = from p in context.Employees.Include(e => e.Orders)
                            where p.EmployeeID.Contains(ID)
                            select new
                            {
                                p.EmployeeID,
                                p.NameEmployee,
                                p.Birthday,
                                p.Gender,
                                p.AddressEmployee,
                                p.PhoneNumber,
                                p.RoleEmployee,
                                p.Active,
                                p.PassWordAccount,
                                Tong = p.Orders.Any() ? p.Orders.Sum(o => (int?)o.Total) : 0,

                            };
                return query.ToList<dynamic>();
            }
        }

        // Method to add a new employee
        public bool ThemNhanVien(ref string err, string id, string name, DateTime birthday, string gender, string address, string sdt, string role, int active, string password)
        {
                using (var context = new QLCuaHang())
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            var newEmployee = new Employee
                            {
                                EmployeeID = id,
                                NameEmployee = name,
                                Birthday = birthday.Date,
                                Gender = gender,
                                AddressEmployee = address,
                                PhoneNumber = sdt,
                                RoleEmployee = role,
                                Active = active.ToString(),
                                PassWordAccount = password
                            };

                            context.Employees.Add(newEmployee);
                            context.SaveChanges();
                            transaction.Commit();

                            return true;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            err = ex.Message;
                            return false;
                        }
                    }
                }
            

        }

        // Method to update an employee
        public bool CapNhatNhanVien(ref string err, string id, string name, DateTime birthday, string gender, string address, string sdt, string role, int active, string password)
        {
            using (var context = new QLCuaHang())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var employeeToUpdate = context.Employees.FirstOrDefault(e => e.EmployeeID == id);

                        if (employeeToUpdate != null)
                        {
                            employeeToUpdate.NameEmployee = name;
                            employeeToUpdate.Birthday = birthday.Date;
                            employeeToUpdate.Gender = gender;
                            employeeToUpdate.AddressEmployee = address;
                            employeeToUpdate.PhoneNumber = sdt;
                            employeeToUpdate.RoleEmployee = role;
                            employeeToUpdate.Active = active.ToString();
                            employeeToUpdate.PassWordAccount = password;

                            context.SaveChanges();
                            transaction.Commit();

                            return true;
                        }
                        else
                        {
                            err = "Employee not found.";
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        err = ex.Message; 
                        return false;
                    }
                }
            }
        }

        public bool XoaNhanVien(ref string err, string id)
        {
            using (var context = new QLCuaHang())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var employeeToUpdate = context.Employees.FirstOrDefault(e => e.EmployeeID == id);

                        if (employeeToUpdate != null)
                        {
                            employeeToUpdate.Active = "0";
                            context.SaveChanges();
                            transaction.Commit();
                            return true;
                        }
                        else
                        {
                            err = "Employee not found.";
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        err = ex.Message;
                        return false;
                    }
                }
            }
        }
    }
}
