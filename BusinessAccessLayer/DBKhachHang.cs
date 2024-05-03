using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
//
using DataAccessLayer; // Importing the namespace for DataAccessLayer
using System.Data.Entity;
using System.Security.Policy;
using System.Xml.Linq;
using DataAccessLayer.Entities;

namespace BusinessAccessLayer
{
    public class DBKhachHang
    {
        DAL db = null; // Declaring an instance of the DAL class
        public DBKhachHang()
        {
            db = new DAL(); // Initializing the instance of the DAL class
        }

        // Method to retrieve all customers
        public List<dynamic> LayKhachHang()
        {
            using (var context = new QLCuaHang())
            {
                var query = (from c in context.Customers.Include(p => p.Orders)
                             select new
                             {
                                 c.PhoneNumber,
                                 c.NameCustomer,
                                 c.Birthday,
                                 c.Gender,
                                 c.Point,
                                 Tong = c.Orders.Any() ? c.Orders.Sum(o => (int?)o.Total) : 0 
                             });

                return query.ToList<dynamic>();
            }
        }

        // Method to search for customers by phone number and name
        public List<dynamic> TimKhachHang(string Phone, string Name)
        {
            using(var context = new QLCuaHang())
            {
                var query = (from c in context.Customers.Include(p => p.Orders)
                             where c.NameCustomer.ToLower().Contains(Name) && c.PhoneNumber.Contains(Phone)
                             select new
                             {
                                 c.PhoneNumber,
                                 c.NameCustomer,
                                 c.Birthday,
                                 c.Gender,
                                 c.Point,
                                 Tong = c.Orders.Any() ? c.Orders.Sum(o => (int?)o.Total) : 0
                             });

                return query.ToList<dynamic>();
            }
            
        }

        // Method to retrieve products purchased by a customer
        public DataSet cuaKhachHang(string Phone)
        {
            return db.ExecuteQueryDataSet("SELECT * FROM ProductOfCustomer('" + Phone + "')",
                CommandType.Text, null); // Executing a SQL query to retrieve products purchased by a customer
        }
        public List<Product> SPcuaKhachHang(string phoneNumber)
        {
            using (var context = new QLCuaHang())
            {
                var query = from od in context.OrderDetails
                            join o in context.Orders on od.Order_ID equals o.Order_ID
                            join p in context.Products on od.Product_ID equals p.Product_ID
                            where o.PhoneNumber == phoneNumber
                            group new { od, p } by new { p.Product_ID, p.ProductName, p.UnitPrice } into grouped
                            select new
                            {
                                Product_ID = grouped.Key.Product_ID,
                                ProductName = grouped.Key.ProductName,
                                Quantity = grouped.Sum(x => x.od.Quantity),
                                UnitPrice = grouped.Key.UnitPrice
                            };

                var result = query.ToList().Select(x => new Product
                {
                    Product_ID = x.Product_ID,
                    ProductName = x.ProductName,
                    Quantity = x.Quantity,
                    UnitPrice = x.UnitPrice
                }).ToList();

                return result;
            }
        }


        // Method to insert a new customer
        public bool ThemKhachHang(ref string err, string sdt, string name, DateTime birthday, string gender, int point)
        {
            using (var context = new QLCuaHang())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        if (string.IsNullOrEmpty(name))
                        {
                            throw new ArgumentException("Vui lòng nhập chính xác, đầy đủ thông tin");
                        }
                        var customer = new Customer
                        {
                            PhoneNumber = sdt,
                            NameCustomer = name,
                            Birthday = birthday.Date,
                            Gender = gender,
                            Point = point
                        };

                        context.Customers.Add(customer);
                        context.SaveChanges();
                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return false;
                        throw new Exception("Lỗi: " + ex.Message);

                    }
                }
            }
        }
        public bool CapNhatKhachHang(ref string err, string sdt, string name, DateTime birthday, string gender, int point)
        {
            using (var context = new QLCuaHang())
            {
                try
                {
                    // Retrieve the customer entity from the database
                    var customer = context.Customers.FirstOrDefault(c => c.PhoneNumber == sdt);

                    if (customer == null)
                    {
                        err = "Không tìm thấy khách hàng có số điện thoại này.";
                        return false;
                    }

                    // Update customer properties
                    customer.NameCustomer = name;
                    customer.Birthday = birthday.Date; // Use only the date part of the birthday
                    customer.Gender = gender;
                    customer.Point = point;

                    // Save changes to the database
                    context.SaveChanges();

                    return true;
                }
                catch (Exception ex)
                {
                    err = "Lỗi: " + ex.Message;
                    return false;
                }
            }
        }
    }
}