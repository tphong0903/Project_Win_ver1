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

namespace BusinessAccessLayer
{
    public class DBKhachHang
    {
        DAL db = null; // Declaring an instance of the DAL class
        public DBKhachHang()
        {
            db = new DAL(); // Initializing the instance of the DAL class
        }
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

        public List<Product> SPcuaKhachHang(string phoneNumber)
        {
            using (var context = new QLCuaHang())
            {
                var query = from od in context.OrderDetails
                            where od.Order.Customer.PhoneNumber == phoneNumber
                            group od by new { od.Product.Product_ID, od.Product.ProductName, od.Product.UnitPrice } into grouped
                            select new
                            {
                                Product_ID = grouped.Key.Product_ID,
                                ProductName = grouped.Key.ProductName,
                                Quantity = grouped.Sum(x => x.Quantity),
                                UnitPrice = grouped.Key.UnitPrice
                            };
                var result = query.ToList().Select(x => new Product
                {
                    Product_ID = x.Product_ID,
                    ProductName = x.ProductName,
                    Quantity = x.Quantity,
                }).ToList();

                return result;
            }
        }

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
                        err = "Lỗi: " + ex.Message;
                        return false;

                    }
                }
            }
        }
        public bool CapNhatKhachHang(ref string err, string sdt, string name, DateTime birthday, string gender, int point)
        {
            using (var context = new QLCuaHang())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var customer = context.Customers.FirstOrDefault(c => c.PhoneNumber == sdt);

                        if (customer == null)
                        {
                            err = "Không tìm thấy khách hàng có số điện thoại này.";
                            return false;
                        }

                        customer.NameCustomer = name;
                        customer.Birthday = birthday.Date; 
                        customer.Gender = gender;
                        customer.Point = point;

                        // Save changes to the database
                        context.SaveChanges();
                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        err = "Lỗi: " + ex.Message;
                        return false;
                    }
                }
            }
        }
    }
}