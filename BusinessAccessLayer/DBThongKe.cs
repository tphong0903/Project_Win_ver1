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
    public class DBThongKe : DAL
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
                NumCustommers = context.Customers.Count();
                NumSuppliers = context.Suppliers.Count();
                NumProduct = context.Products.Count();
                NumOrder = context.Orders.Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate).Count();
            }

        }

        // Method to perform analysis on orders
        private void GetOrderAnalisys()
        {
            using (var context = new QLCuaHang())
            {
                GrossRevenueList = new List<RevenueByDate>();

                // Initializing total profit and total revenue
                TotalProfit = 0;
                TotalRevenue = 0;
                var reader = (from order in context.Orders
                              where order.OrderDate >= startDate && order.OrderDate <= endDate
                              group order by order.OrderDate into g
                              select new
                              {
                                  Date = g.Key,
                                  TotalAmount = g.Sum(o => o.Total)
                              }).ToList();
                var resultTable = new List<KeyValuePair<DateTime, int>>();
                foreach (var data in reader)
                {
                    resultTable.Add(new KeyValuePair<DateTime, int>(data.Date, data.TotalAmount));
                    TotalRevenue += data.TotalAmount;
                }

                var reader1 = from od in context.OrderDetails
                              join p in context.Products on od.Product_ID equals p.Product_ID
                              join id in context.ImportDetails on od.Product_ID equals id.Product_ID
                              join o in context.Orders on od.Order_ID equals o.Order_ID
                              where o.OrderDate >= startDate && o.OrderDate <= endDate
                              group new { od, p, id, o } by o.OrderDate into g
                              select new
                              {
                                  OrderDate = g.Key,
                                  TotalProfit = g.Sum(item => (item.p.UnitPrice - item.id.Unitcost) * item.od.Quantity)
                              };
                foreach (var item in reader1)
                {
                    decimal unitProfit = Convert.ToDecimal(item.TotalProfit);
                    TotalProfit += unitProfit;
                }

                // Grouping revenue by date
                if (numberDays <= 1) // Group by hour
                {
                    GrossRevenueList = (from orderList in resultTable
                                        group orderList by orderList.Key.ToString("hh tt")
                                        into order
                                        select new RevenueByDate
                                        {
                                            Date = order.Key,
                                            TotalAmount = order.Sum(amount => amount.Value)
                                        }).ToList();
                }
                else if (numberDays <= 30) // Group by day
                {
                    GrossRevenueList = (from orderList in resultTable
                                        group orderList by orderList.Key.ToString("dd MMM")
                                        into order
                                        select new RevenueByDate
                                        {
                                            Date = order.Key,
                                            TotalAmount = order.Sum(amount => amount.Value)
                                        }).ToList();
                }
                else if (numberDays <= 92) // Group by week
                {
                    GrossRevenueList = (from orderList in resultTable
                                        group orderList by CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                                            orderList.Key, CalendarWeekRule.FirstDay, DayOfWeek.Monday)
                                        into order
                                        select new RevenueByDate
                                        {
                                            Date = "Week " + order.Key.ToString(),
                                            TotalAmount = order.Sum(amount => amount.Value)
                                        }).ToList();
                }
                else if (numberDays <= (365 * 2)) // Group by month
                {
                    bool isYear = numberDays <= 365 ? true : false;
                    GrossRevenueList = (from orderList in resultTable
                                        group orderList by orderList.Key.ToString("MMM yyyy")
                                        into order
                                        select new RevenueByDate
                                        {
                                            Date = isYear ? order.Key.Substring(0, order.Key.IndexOf(" ")) : order.Key,
                                            TotalAmount = order.Sum(amount => amount.Value)
                                        }).ToList();
                }
                else // Group by year
                {
                    GrossRevenueList = (from orderList in resultTable
                                        group orderList by orderList.Key.ToString("yyyy")
                                        into order
                                        select new RevenueByDate
                                        {
                                            Date = order.Key,
                                            TotalAmount = order.Sum(amount => amount.Value)
                                        }).ToList();
                }
            }
        }
        // Method to perform analysis on products
        private void GetProductAnalisys()
        {
            TopProductsList = new List<KeyValuePair<string, int>>();
            UnderstocksList = new List<KeyValuePair<string, int>>();
            using (var context = new QLCuaHang())
            {
                var topProductsList = (from od in context.OrderDetails
                                       join p in context.Products on od.Product_ID equals p.Product_ID
                                       join o in context.Orders on od.Order_ID equals o.Order_ID
                                       where o.OrderDate >= startDate && o.OrderDate <= endDate
                                       group od by p.ProductName into g
                                       orderby g.Sum(od => od.Quantity) descending
                                       select new
                                       {
                                           ProductName = g.Key,
                                           Quantity = g.Sum(od => od.Quantity)
                                       }).Take(5).ToList();

                foreach (var item in topProductsList)
                {
                    TopProductsList.Add(new KeyValuePair<string, int>(item.ProductName, item.Quantity));
                }

                var understockedList = context.Products.Where(p => p.Quantity <= 6).Select(p => new { p.ProductName, p.Quantity }).ToList();

                foreach (var item in understockedList)
                {
                    UnderstocksList.Add(new KeyValuePair<string, int>(item.ProductName, item.Quantity));
                }
            }

        }

        // Method to load data for analysis within a specified date range
        public bool LoadData(DateTime startDate, DateTime endDate)
        {
            // Setting the end date to the last second of the day
            endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day,
                endDate.Hour, endDate.Minute, 59);

            // Checking if the start date or end date has changed
            if (startDate != this.startDate || endDate != this.endDate)
            {
                // Updating instance variables
                this.startDate = startDate;
                this.endDate = endDate;
                this.numberDays = (endDate - startDate).Days;

                // Performing data analysis
                GetNumberItems();
                GetProductAnalisys();
                GetOrderAnalisys();

                // Logging the refresh status
                Console.WriteLine("Refreshed data: {0} - {1}", startDate.ToString(), endDate.ToString());
                return true;
            }
            else
            {
                // Logging that data is not refreshed
                Console.WriteLine("Data not refreshed, same query: {0} - {1}", startDate.ToString(), endDate.ToString());
                return false;
            }
        }
    }
}
