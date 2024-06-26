﻿using System;
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
     
        public DBNhaCungCap()
        {
           
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
                var query = from id in context.ImportDetails.Include(i=>i.Import)
                            where id.Import.Supplier_ID == ID
                            group id by new { id.Product_ID, id.Product.ProductName, id.Unitcost, id.Quantity } into grouped
                            select new
                            {
                                Product_ID = grouped.Key.Product_ID,
                                ProductName = grouped.Key.ProductName,
                                Quantity = grouped.Sum(x => x.Quantity),
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
                        var supplierToUpdate = context.Suppliers.FirstOrDefault(s => s.Supplier_ID == Supplier_ID);

                        if (supplierToUpdate != null)
                        {
                            supplierToUpdate.CompanyName = CompanyName;
                            supplierToUpdate.PhoneNumber = PhoneNumber;
                            supplierToUpdate.AddressSupplier = AddressSupplier;
                            supplierToUpdate.Email = Email;

                            context.SaveChanges();

                            transaction.Commit();

                            return true;
                        }
                        else
                        {
                            err = "Supplier not found.";
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        err = "Error: " + ex.Message; 
                        return false; 
                    }
                }
            }
        }
    }
}
