using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer;
using static System.Data.Entity.Infrastructure.Design.Executor;
using System.Data.Entity;
using DataAccessLayer.Entities;
using System.Data.Entity.Core.Objects;
using System.Runtime.Remoting.Messaging;

namespace BusinessAccessLayer
{
    // Defining a struct to represent revenue by date
    public struct RevenueByDate
    {
        public string Date { get; set; } // Date property
        public int TotalAmount { get; set; } // TotalAmount property
    }

    // Defining the DBThongKe class inheriting from the DAL class
    public class DBThongKe
    {
        private DateTime startDate;
        private DateTime endDate;
        private int numberDays;

        // Declaring properties
        public int NumCustommers { get; private set; }
        public int NumSuppliers { get; private set; }
        public int NumProduct { get; private set; }
        public List<KeyValuePair<string, int>> TopProductsList { get; private set; }
        public List<KeyValuePair<string, int>> UnderstocksList { get; private set; }
        public List<RevenueByDate> GrossRevenueList { get; private set; }
        public int NumOrder { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalProfit { get; set; }

        // Constructor for the DBThongKe class
        public DBThongKe()
        {
            

        }

        // Method to get the number of customers, suppliers, and products
        private void GetNumberItems()
        {
            using (var context = new QLCuaHang())
            {
                // Đếm số lượng khách hàng, nhà cung cấp, sản phẩm và đơn hàng trong khoảng thời gian cụ thể
                NumCustommers = context.Customers.Count();
                NumSuppliers = context.Suppliers.Count();
                NumProduct = context.Products.Count();
                NumOrder = context.Orders.Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate).Count();
            }
        }

        // Method to perform analysis on orders
        private void GetOrderAnalisys()
        {
            // Sử dụng context để truy cập cơ sở dữ liệu
            using (var context = new QLCuaHang())
            {
                // Khởi tạo danh sách GrossRevenueList để lưu trữ doanh thu theo ngày
                GrossRevenueList = new List<RevenueByDate>();

                // Khởi tạo biến tổng lợi nhuận và tổng doanh thu
                TotalProfit = 0;
                TotalRevenue = 0;

                // Truy vấn lấy tổng doanh thu từ các đơn hàng trong khoảng thời gian startDate đến endDate
                var reader = (from order in context.Orders
                              where order.OrderDate >= startDate && order.OrderDate <= endDate
                              group order by order.OrderDate into g
                              select new
                              {
                                  Date = g.Key, // Ngày của đơn hàng
                                  TotalAmount = g.Sum(o => o.Total) // Tổng doanh thu của nhóm đơn hàng này
                              }).ToList();

                // Khởi tạo danh sách resultTable để lưu trữ kết quả của truy vấn
                var resultTable = new List<KeyValuePair<DateTime, int>>();

                // Duyệt qua các kết quả của truy vấn để tính tổng doanh thu và thêm vào resultTable
                foreach (var data in reader)
                {
                    resultTable.Add(new KeyValuePair<DateTime, int>(data.Date, data.TotalAmount));
                    TotalRevenue += data.TotalAmount; // Tổng doanh thu của toàn bộ các đơn hàng trong khoảng thời gian
                }

                // Truy vấn lấy thông tin chi tiết đơn hàng để tính lợi nhuận từ các sản phẩm
                var reader1 = from od in context.OrderDetails
                              join p in context.Products on od.Product_ID equals p.Product_ID
                              join id in context.ImportDetails on od.Product_ID equals id.Product_ID
                              join o in context.Orders on od.Order_ID equals o.Order_ID
                              where o.OrderDate >= startDate && o.OrderDate <= endDate
                              group new { od, p, id, o } by o.OrderDate into g
                              select new
                              {
                                  OrderDate = g.Key, // Ngày của đơn hàng
                                  TotalProfit = g.Sum(item => (item.p.UnitPrice - item.id.Unitcost) * item.od.Quantity) // Tổng lợi nhuận từ các sản phẩm trong đơn hàng
                              };

                // Duyệt qua các kết quả của truy vấn reader1 để tính tổng lợi nhuận và cập nhật vào TotalProfit
                foreach (var item in reader1)
                {
                    decimal unitProfit = Convert.ToDecimal(item.TotalProfit);
                    TotalProfit += unitProfit; // Tổng lợi nhuận của toàn bộ các đơn hàng trong khoảng thời gian
                }

                // Nhóm doanh thu theo ngày theo các khoảng thời gian khác nhau
                if (numberDays <= 1) // Nhóm theo giờ
                {
                    GrossRevenueList = (from orderList in resultTable
                                        group orderList by orderList.Key.ToString("hh tt") // Nhóm theo giờ (hh tt là định dạng giờ trong AM/PM)
                                        into order
                                        select new RevenueByDate
                                        {
                                            Date = order.Key,
                                            TotalAmount = order.Sum(amount => amount.Value) // Tổng doanh thu của nhóm giờ này
                                        }).ToList();
                }
                else if (numberDays <= 30) // Nhóm theo ngày
                {
                    GrossRevenueList = (from orderList in resultTable
                                        group orderList by orderList.Key.ToString("dd MMM") // Nhóm theo ngày (dd MMM là định dạng ngày tháng)
                                        into order
                                        select new RevenueByDate
                                        {
                                            Date = order.Key,
                                            TotalAmount = order.Sum(amount => amount.Value) // Tổng doanh thu của nhóm ngày này
                                        }).ToList();
                }
                else if (numberDays <= 92) // Nhóm theo tuần
                {
                    GrossRevenueList = (from orderList in resultTable
                                        group orderList by CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                                            orderList.Key, CalendarWeekRule.FirstDay, DayOfWeek.Monday) // Nhóm theo tuần
                                        into order
                                        select new RevenueByDate
                                        {
                                            Date = "Week " + order.Key.ToString(),
                                            TotalAmount = order.Sum(amount => amount.Value) // Tổng doanh thu của nhóm tuần này
                                        }).ToList();
                }
                else if (numberDays <= (365 * 2)) // Nhóm theo tháng
                {
                    bool isYear = numberDays <= 365 ? true : false; // Xác định có phải là năm đầu tiên hay không
                    GrossRevenueList = (from orderList in resultTable
                                        group orderList by orderList.Key.ToString("MMM yyyy") // Nhóm theo tháng (MMM yyyy là định dạng tháng năm)
                                        into order
                                        select new RevenueByDate
                                        {
                                            Date = isYear ? order.Key.Substring(0, order.Key.IndexOf(" ")) : order.Key,
                                            TotalAmount = order.Sum(amount => amount.Value) // Tổng doanh thu của nhóm tháng này
                                        }).ToList();
                }
                else // Nhóm theo năm
                {
                    GrossRevenueList = (from orderList in resultTable
                                        group orderList by orderList.Key.ToString("yyyy") // Nhóm theo năm (yyyy là định dạng năm)
                                        into order
                                        select new RevenueByDate
                                        {
                                            Date = order.Key,
                                            TotalAmount = order.Sum(amount => amount.Value) // Tổng doanh thu của nhóm năm này
                                        }).ToList();
                }
            }
        }

        // Method to perform analysis on products
        private void GetProductAnalisys()
        {
            // Khởi tạo danh sách TopProductsList và UnderstocksList để lưu trữ thông tin về sản phẩm
            TopProductsList = new List<KeyValuePair<string, int>>();
            UnderstocksList = new List<KeyValuePair<string, int>>();

            // Sử dụng context để truy cập cơ sở dữ liệu
            using (var context = new QLCuaHang())
            {
                // Truy vấn lấy danh sách các sản phẩm bán chạy nhất trong khoảng thời gian startDate đến endDate
                var topProductsList = (from od in context.OrderDetails
                                       join p in context.Products on od.Product_ID equals p.Product_ID
                                       join o in context.Orders on od.Order_ID equals o.Order_ID
                                       where o.OrderDate >= startDate && o.OrderDate <= endDate
                                       group od by p.ProductName into g
                                       orderby g.Sum(od => od.Quantity) descending
                                       select new
                                       {
                                           ProductName = g.Key, // Tên sản phẩm
                                           Quantity = g.Sum(od => od.Quantity) // Tổng số lượng bán được của sản phẩm này
                                       }).Take(5).ToList(); // Lấy ra 5 sản phẩm có lượng bán nhiều nhất

                // Duyệt qua danh sách topProductsList để thêm vào TopProductsList
                foreach (var item in topProductsList)
                {
                    TopProductsList.Add(new KeyValuePair<string, int>(item.ProductName, item.Quantity));
                }

                // Truy vấn lấy danh sách các sản phẩm trong tình trạng thiếu hàng (số lượng dưới 6)
                var understockedList = context.Products.Where(p => p.Quantity <= 6).Select(p => new { p.ProductName, p.Quantity }).ToList();

                // Duyệt qua danh sách understockedList để thêm vào UnderstocksList
                foreach (var item in understockedList)
                {
                    UnderstocksList.Add(new KeyValuePair<string, int>(item.ProductName, item.Quantity));
                }
            }
        }

        // Method to load data for analysis within a specified date range
        public bool LoadData(DateTime startDate, DateTime endDate)
        {
            // Đặt ngày kết thúc là giây cuối cùng của ngày đó
            endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day,
                endDate.Hour, endDate.Minute, 59);

            // Kiểm tra xem ngày bắt đầu hoặc ngày kết thúc đã thay đổi so với trước đó
            if (startDate != this.startDate || endDate != this.endDate)
            {
                // Cập nhật các biến thể hiện
                this.startDate = startDate;
                this.endDate = endDate;
                this.numberDays = (endDate - startDate).Days; // Tính số ngày trong khoảng thời gian

                // Thực hiện phân tích dữ liệu
                GetNumberItems(); // Lấy thông tin số lượng mặt hàng
                GetProductAnalisys(); // Phân tích sản phẩm
                GetOrderAnalisys(); // Phân tích đơn hàng

                // Ghi nhật ký tình trạng làm mới dữ liệu
                Console.WriteLine("Đã làm mới dữ liệu: {0} - {1}", startDate.ToString(), endDate.ToString());
                return true;
            }
            else
            {
                // Ghi nhật ký rằng dữ liệu không được làm mới do trùng lặp truy vấn
                Console.WriteLine("Dữ liệu không được làm mới, truy vấn giống nhau: {0} - {1}", startDate.ToString(), endDate.ToString());
                return false;
            }
        }
    }
}
