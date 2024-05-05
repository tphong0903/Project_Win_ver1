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

namespace BusinessAccessLayer // Declaring the BusinessAccessLayer namespace
{
    public class DBBienLai // Declaring the DBBienLai class
    {
     
        public DBBienLai() // Constructor for the DBBienLai class
        {
           
        }

        // Method to retrieve receipts
        public List<dynamic> LayBienLai()
        {
            using (var context = new QLCuaHang())
            {
                var query = from import in context.Imports.Include(o=>o.Supplier)
                            select new
                            {
                                import.Import_ID,
                                import.ImportDay,
                                import.Total,
                                import.Supplier_ID,
                                import.Supplier.CompanyName
                            };
                return query.ToList<dynamic>();
            }
        }

        // Method to search for receipts by ID and date
        public List<dynamic> TimBienLai(string HD, string MKH)
        {
            using (var context = new QLCuaHang())
            {

                var query = from import in context.Imports
                            .Include(o => o.Supplier)
                            where import.Import_ID.ToLower().Contains(HD) && import.Supplier_ID.Contains(MKH)
                            select new
                            {
                                import.Import_ID,
                                import.ImportDay,
                                import.Total,
                                import.Supplier_ID,
                                import.Supplier.CompanyName
                            };
                return query.ToList<dynamic>();
            }
        }

        public List<dynamic> SPCuaBienLai(string HD)
        {
            using (var context = new QLCuaHang())
            {

                var query = (from import in context.ImportDetails.Include(o => o.Product)
                             where import.Import_ID.ToLower().Contains(HD)
                             select new
                             {
                                 import.Product_ID,
                                 import.Product.ProductName,
                                 import.Quantity,
                                 import.Unitcost
                             });
                return query.ToList<dynamic>();
            }
        }

        public bool ThemBienLai(ref string err, string Import_ID, string Supplier_ID, DateTime ImportDay, int Total)
        {
            using (var context = new QLCuaHang())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        if (string.IsNullOrEmpty(Supplier_ID))
                        {
                            throw new ArgumentException("Vui lòng nhập chính xác, đầy đủ thông tin");
                        }

                        var import = new Import
                        {
                            Import_ID = Import_ID,
                            Supplier_ID = Supplier_ID,
                            ImportDay = ImportDay.Date,
                            Total = Total
                        };

                        context.Imports.Add(import);
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
        public bool ThemChiTietBienLai(ref string err, string Import_ID, string Product_ID,int Quantity, int Unitcost)
        {
            try
            {
                if (string.IsNullOrEmpty(Import_ID) || string.IsNullOrEmpty(Product_ID) || Quantity <= 0 || Unitcost <= 0)
                {
                    throw new ArgumentException("Vui lòng nhập đầy đủ và chính xác thông tin chi tiết biên lai");
                }

                using (var context = new QLCuaHang())
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            var importDetail = new ImportDetail
                            {
                                Import_ID = Import_ID,
                                Product_ID = Product_ID,
                                Quantity = Quantity,
                                Unitcost = Unitcost
                            };

                            var product = context.Products.FirstOrDefault(p => p.Product_ID == Product_ID);
                            var import = context.Imports.FirstOrDefault(i => i.Import_ID == Import_ID);

                            if (product != null && import != null)
                            {
                                context.ImportDetails.Add(importDetail);
                                product.Quantity += Quantity;
                                import.Total += Quantity * Unitcost;

                                context.SaveChanges();
                                transaction.Commit();
                                return true;
                            }
                            else
                            {
                                throw new InvalidOperationException("Không tìm thấy sản phẩm hoặc phiếu nhập phù hợp");
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
    }
}
