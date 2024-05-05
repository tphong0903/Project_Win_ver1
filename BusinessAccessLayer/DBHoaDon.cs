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
using System.Runtime.Remoting.Contexts;

namespace BusinessAccessLayer // Declaring the BusinessAccessLayer namespace
{
    public class DBHoaDon // Declaring the DBHoaDon class
    {
        DAL db = null; // Declaring an instance of the DAL class and initializing it to null
        public DBHoaDon() // Constructor for the DBHoaDon class
        {
            db = new DAL(); // Initializing the db instance with a new instance of the DAL class
            using(var context = new QLCuaHang())
            {
                var ordersWithDetails = context.Orders.Include(o => o.OrderDetails).ToList();
                foreach (var order in ordersWithDetails)
                {
                    order.Total = order.OrderDetails.Sum(d => d.Quantity * d.Product.UnitPrice);

                }
                context.SaveChanges();
            }
        }

        // Method to retrieve bills
        public List<dynamic> LayHoaDon()
        {
            using (var context = new QLCuaHang())
            {
                var query = from order in context.Orders
                            .Include(o => o.Employee)
                            .Include(o => o.Customer)
                            .Include(o => o.Discount)
                            select new
                            {
                                order.Order_ID,
                                order.Customer.NameCustomer,
                                order.Employee.NameEmployee,
                                order.OrderDate,
                                order.Total,
                                PercentageDiscount = order.Discount.DiscountCode != null ? order.Discount.PercentageDiscount : 0,
                                order.PhoneNumber,
                                DiscountCode = order.Discount.DiscountCode != null ? order.Discount.DiscountCode : "",
                                order.EmployeeID
                            };
                
                return query.ToList<dynamic>();
            }
        }
        public int LayGiamGia(string discountCode)
        {
            using (var context = new QLCuaHang())
            {
                var discounts = context.Discounts.Where(d => d.DiscountCode == discountCode).Select(p => p.PercentageDiscount).FirstOrDefault();

                return discounts;
            }
        }
        public List<dynamic> TimHoaDon(string HD, string MKH)
        {
            using (var context = new QLCuaHang())
            {
                
                var query = from order in context.Orders
                            .Include(o => o.Employee)
                            .Include(o => o.Customer)
                            .Include(o => o.Discount)
                            where order.Order_ID.ToLower().Contains(HD) && order.PhoneNumber.Contains(MKH)
                            select new
                            {
                                order.Order_ID,
                                order.Customer.NameCustomer,
                                order.Employee.NameEmployee,
                                order.OrderDate,
                                order.Total,
                                order.Discount.PercentageDiscount,
                                order.PhoneNumber,
                                DiscountCode = order.Discount != null ? order.Discount.DiscountCode : "",
                                order.EmployeeID
                            };
                return query.ToList<dynamic>();
            }
        }

        // Method to retrieve products associated with a bill
        public List<dynamic> SPCuaHoaDon(string HD)
        {
            using (var context = new QLCuaHang())
            {

                var query = (from order in context.OrderDetails.Include(o=>o.Product)
                            where order.Order_ID.ToLower().Contains(HD)
                            select new
                            {
                                order.Product_ID,
                                order.Product.ProductName,
                                order.Quantity,
                            });
                return query.ToList<dynamic>();
            }
        }

        // Method to add a new bill
        public bool ThemHoaDon(ref string err, string order_ID, string sdt, string nv,DateTime orderdate, int Total, string magiam)
        {
            using (var context = new QLCuaHang())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        if (string.IsNullOrEmpty(sdt))
                        {
                            throw new ArgumentException("Vui lòng nhập chính xác, đầy đủ thông tin");
                        }

                        var order = new Order
                        {
                            Order_ID = order_ID,
                            PhoneNumber = sdt,
                            EmployeeID = nv,
                            OrderDate = orderdate,
                            Total = Total,
                            DiscountCode = string.IsNullOrEmpty(magiam) ? null : magiam
                        };

                        context.Orders.Add(order);
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

        public bool ThemChiTietHoaDon(ref string err, string Order_ID, string Product_ID,int Quantity)
        {
            using (var context = new QLCuaHang())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        if (string.IsNullOrEmpty(Product_ID))
                        {
                            throw new ArgumentException("Vui lòng nhập chính xác, đầy đủ thông tin");
                        }

                        var orderDetail = new OrderDetail
                        {
                            Order_ID = Order_ID,
                            Product_ID = Product_ID,
                            Quantity = Quantity
                        };

                        context.OrderDetails.Add(orderDetail);
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
        public bool XoaHoaDon(ref string err, string Order_ID)
        {
            using (var context = new QLCuaHang())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var order = context.Orders.FirstOrDefault(o => o.Order_ID == Order_ID);
                        if (order == null)
                        {
                            err = "Không tìm thấy đơn hàng có ID " + Order_ID;
                            return false;
                        }

                        context.Orders.Remove(order);
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
