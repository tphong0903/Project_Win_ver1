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
    public class DBNhaCungCap // Declaring the DBNhaCungCap class
    {
        DAL db = null;

        public DBNhaCungCap()
        {
            db = new DAL();
        }
        public List<dynamic> LayNhaCungCap()
        {
            using (var context = new QLCuaHang())
            {
                var query = from c in context.Suppliers.Include(p => p.Imports)
                            select new
                            {
                                c.Supplier_ID,
                                c.CompanyName,
                                c.PhoneNumber,
                                c.AddressSupplier,
                                c.Email,
                                Tong = c.Imports.Any() ? c.Imports.Sum(o => (int)o.Total) : 0
                            };
                return query.ToList<dynamic>();
            }
        }
        public List<dynamic> TimNhaCungCap(string ID, string name)
        {
            using (var context = new QLCuaHang())
            {
                var query = from c in context.Suppliers.Include(p => p.Imports)
                            where c.CompanyName.Contains(name) && c.Supplier_ID.Contains(ID)
                            select new
                            {
                                c.Supplier_ID,
                                c.CompanyName,
                                c.PhoneNumber,
                                c.AddressSupplier,
                                c.Email,
                                Tong = c.Imports.Any() ? c.Imports.Sum(o => (int)o.Total) : 0
                            };

                return query.ToList<dynamic>();
            }
        }
        public List<dynamic> SPCuaNhaCungCap(string ID)
        {

            using (var context = new QLCuaHang())
            {
                var query = from id in context.ImportDetails
                            join p in context.Products on id.Product_ID equals p.Product_ID
                            join i in context.Imports on id.Import_ID equals i.Import_ID
                            where i.Supplier_ID == ID
                            group new { id, p } by new { id.Product_ID, p.ProductName, id.Unitcost } into grouped
                            select new
                            {
                                Product_ID = grouped.Key.Product_ID,
                                ProductName = grouped.Key.ProductName,
                                Quantity = grouped.Sum(x => x.id.Quantity),
                                Unitcost = grouped.Key.Unitcost
                            };

                return query.ToList<dynamic>();
            }

        }

        // Method to add a new supplier
        public bool ThemNhaCungCap(ref string err, string Supplier_ID, string CompanyName, string PhoneNumber, string AddressSupplier, string Email)
        {
            using (var context = new QLCuaHang())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        if (string.IsNullOrEmpty(CompanyName))
                        {
                            err = "Lỗi: Vui lòng nhập chính xác, đầy đủ thông tin";
                            return false; ;
                        }

                        var supplier = new Supplier
                        {
                            Supplier_ID = Supplier_ID,
                            CompanyName = CompanyName,
                            PhoneNumber = PhoneNumber,
                            AddressSupplier = AddressSupplier,
                            Email = Email
                        };

                        context.Suppliers.Add(supplier);
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

        // Method to update a supplier
        public bool CapNhatNhaCungCap(ref string err, string Supplier_ID, string CompanyName, string PhoneNumber, string AddressSupplier, string Email)
        {
            using (var context = new QLCuaHang())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        // Retrieve the supplier to update
                        var supplierToUpdate = context.Suppliers.FirstOrDefault(s => s.Supplier_ID == Supplier_ID);

                        if (supplierToUpdate != null)
                        {
                            // Update the supplier properties
                            supplierToUpdate.CompanyName = CompanyName;
                            supplierToUpdate.PhoneNumber = PhoneNumber;
                            supplierToUpdate.AddressSupplier = AddressSupplier;
                            supplierToUpdate.Email = Email;

                            // Save changes to the database
                            context.SaveChanges();

                            transaction.Commit(); // Commit the transaction

                            return true; // Successful update
                        }
                        else
                        {
                            err = "Supplier not found."; // Set error message
                            return false; // Supplier not found
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback(); // Rollback the transaction
                        err = "Error: " + ex.Message; // Set error message
                        return false; // Failed update
                    }
                }
            }
        }
    }
}
