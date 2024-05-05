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
        
        public DBHoaDon() // Constructor for the DBHoaDon class
        {
            
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
        public bool KiemTraNgayThangHopLe(string discountCode, DateTime orderdate)
        {
            using (var context = new QLCuaHang())
            {
                var discount = context.Discounts.FirstOrDefault(d => d.DiscountCode == discountCode);

                if (discount != null)
                {
                    // Kiểm tra ngày bắt đầu và ngày kết thúc của mã giảm giá
                    if (discount.StartDay <= orderdate && orderdate <= discount.EndDay)
                    {
                        return true; // Mã giảm giá hợp lệ về ngày tháng
                    }
                }

                return false; // Mã giảm giá không hợp lệ về ngày tháng
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
                                PercentageDiscount = order.Discount.DiscountCode != null ? order.Discount.PercentageDiscount : 0,
                                order.PhoneNumber,
                                DiscountCode = order.Discount.DiscountCode != null ? order.Discount.DiscountCode : "",
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

                var query = (from order in context.OrderDetails.Include(o => o.Product)
                             where order.Order_ID.ToLower().Contains(HD)
                             select new
                             {
                                 order.Product_ID,
                                 order.Product.ProductName,
                                 order.Quantity,
                                 order.Product.UnitPrice,
                             });
                return query.ToList<dynamic>();
            }
        }

        // Method to add a new bill
        public bool ThemHoaDon(ref string err, string order_ID, string sdt, string nv, DateTime orderdate, int Total, string magiam)
        {
            try
            {
                if (string.IsNullOrEmpty(sdt))
                {
                    throw new ArgumentException("Vui lòng nhập chính xác, đầy đủ thông tin");
                }

                using (var context = new QLCuaHang())
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            if (!string.IsNullOrEmpty(magiam) && !KiemTraNgayThangHopLe(magiam, orderdate))
                            {
                                throw new ArgumentException("Mã giảm giá không đúng hạn");
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
            catch (Exception ex)
            {
                err = "Lỗi: " + ex.Message;
                return false;
            }
        }

        public bool ThemChiTietHoaDon(ref string err, string Order_ID, string Product_ID, int Quantity)
        {
            try
            {
                if (string.IsNullOrEmpty(Product_ID))
                {
                    throw new ArgumentException("Vui lòng nhập đầy đủ thông tin sản phẩm");
                }

                using (var context = new QLCuaHang())
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            var orderDetail = new OrderDetail
                            {
                                Order_ID = Order_ID,
                                Product_ID = Product_ID,
                                Quantity = Quantity
                            };

                            var product = context.Products.FirstOrDefault(p => p.Product_ID == Product_ID);
                            var order = context.Orders.FirstOrDefault(o => o.Order_ID == Order_ID);

                            if (order != null && product != null)
                            {
                                int discountPercentage = 0;
                                if (!string.IsNullOrEmpty(order.DiscountCode))
                                {
                                    discountPercentage = LayGiamGia(order.DiscountCode);
                                }

                                if (product.Quantity >= orderDetail.Quantity)
                                {
                                    product.Quantity -= orderDetail.Quantity;
                                    order.Total += product.UnitPrice * orderDetail.Quantity * (100 - discountPercentage) / 100;
                                }
                                else
                                {
                                    throw new InvalidOperationException("Số lượng sản phẩm không đủ");
                                }

                                context.OrderDetails.Add(orderDetail);

                                var customer = context.Customers.Include(c => c.Orders).FirstOrDefault(c => c.PhoneNumber == order.PhoneNumber);
                                if (customer != null)
                                {
                                    customer.Point = customer.Orders.Sum(o => o.Total/10000);
                                }

                                context.SaveChanges();
                                transaction.Commit();
                                return true;
                            }
                            else
                            {
                                throw new InvalidOperationException("Không tìm thấy đơn hàng hoặc sản phẩm phù hợp");
                            }
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
            catch (Exception ex)
            {
                err = "Lỗi: " + ex.Message;
                return false;
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
                        var order = context.Orders.Include(o => o.OrderDetails).FirstOrDefault(o => o.Order_ID == Order_ID);
                        if (order == null)
                        {
                            err = "Không tìm thấy đơn hàng có ID " + Order_ID;
                            return false;
                        }

                        int totalValueToRemove = order.Total;

                        foreach (var orderDetail in order.OrderDetails)
                        {
                            var product = context.Products.FirstOrDefault(p => p.Product_ID == orderDetail.Product_ID);
                            if (product != null)
                            {
                                product.Quantity += orderDetail.Quantity;
                            }
                        }

                        var customer = context.Customers.FirstOrDefault(c => c.PhoneNumber == order.PhoneNumber);
                        if (customer != null)
                        {
                            customer.Point -= totalValueToRemove;
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
