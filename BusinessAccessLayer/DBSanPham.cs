using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
//
using DataAccessLayer;
using System.Xml.Linq;
using DataAccessLayer.Entities;
using System.Data.Entity;
using System.Runtime.Remoting.Contexts;

namespace BusinessAccessLayer // Declaring the BusinessAccessLayer namespace
{
    public class DBSanPham // Declaring the DBSanPham class
    {
        DAL db = null; // Declaring an instance of the DAL class and initializing it to null

        // Constructor for the DBSanPham class
        public DBSanPham()
        {
            db = new DAL(); // Initializing the db instance with a new instance of the DAL class

        }
        public List<dynamic> LoadSanPham()
        {
            using (var dataContext = new QLCuaHang())
            {
                var products = from p in dataContext.Products
                               select new
                               {
                                   p.Product_ID,
                                   p.ProductName,
                                   p.UnitPrice,
                                   p.Quantity,
                               };
               

                return products.ToList<dynamic>();
            }
        }
        public List<dynamic> LoadDanhMuc()
        {
            using (var dataContext = new QLCuaHang())
            {
                var products = from p in dataContext.Brands
                               select p.BrandName;

                return products.ToList<dynamic>();
            }
        }
        public List<dynamic> FindSanPham(string keyword, string brand,string cate)
        {
            using (var dataContext = new QLCuaHang())
            {
                var b = (from p in dataContext.Brands where p.BrandName == brand select p.Brand_ID).FirstOrDefault();
                var c = (from p in dataContext.Categories where p.CategoryName == cate select p.Category_ID).FirstOrDefault();
                if(b == null)
                {
                    b = "";
                }
                if (c == null)
                {
                    c= "";
                }
                var products = from p in dataContext.Products
                               where p.ProductName.ToLower().Contains(keyword) && p.Brand_ID.Contains(b) && p.Category_ID.Contains(c)
                               select new
                               {
                                   p.Product_ID,
                                   p.ProductName,
                                   p.UnitPrice,
                                   p.Quantity
                               };

                return products.ToList<dynamic>();
            }
        }
        public List<Product> ChiTietSP(string id)
        {
            using (var dataContext = new QLCuaHang())
            {
                var products = (from p in dataContext.Products
                                where p.Product_ID == id
                                select p)
                               .Include(p => p.Brand) 
                               .Include(p => p.Category) 
                               .Include(p => p.PictureProduct)
                               .ToList();

                return products;
            }
        }
        public List<dynamic> LaySanPham()
        {
            using (var dataContext = new QLCuaHang())
            {
                var products = from p in dataContext.Products
                               select new
                               {
                                   p.Product_ID,
                                   p.ProductName,
                                   p.UnitPrice,
                                   p.Quantity,
                               };


                return products.ToList<dynamic>();
            }
        }
        public List<dynamic> LaySanPhamChoFormBienLai()
        {
            using (var dataContext = new QLCuaHang())
            {
                var products = from p in dataContext.Products
                               select new
                               {
                                   p.Product_ID,
                                   p.ProductName,
                               };


                return products.ToList<dynamic>();
            }
        }

        public bool SuaHinhAnh(string name,int id)
        {
            using (var context = new QLCuaHang())
            {
                var pictureProduct = context.PictureProducts.SingleOrDefault(p => p.Picture_ID == id);

                if (pictureProduct != null)
                {
                    // Cập nhật tên hình ảnh
                    pictureProduct.Picture_Name = name;

                    // Lưu thay đổi vào cơ sở dữ liệu
                    context.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        public bool ThemHinhAnh(string name)
        {
            using(var context = new QLCuaHang())
            {
                var pictureProduct = new PictureProduct { Picture_Name = name };
                if(pictureProduct != null)
                {
                    context.PictureProducts.Add(pictureProduct);
                    context.SaveChanges();
                    return true;
                }
                return false;
                
            }
        }

        // Method to retrieve categories
        public List<dynamic> LayDanhMuc()
        {
            using (var dataContext = new QLCuaHang())
            {
                var products = from p in dataContext.Categories
                               select p;


                return products.ToList<dynamic>();
            }
        }

        // Method to retrieve brands
        public List<dynamic> LayThuongHieu()
        {
            using (var dataContext = new QLCuaHang())
            {
                var products = from p in dataContext.Brands
                               select p;


                return products.ToList<dynamic>();
            }
        }

        // Method to update product details
        public bool CapNhatSanPham(string ma, string ten, int gia, string th, string dm, int sl, int idImg)
        {
            using (var context = new QLCuaHang())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var brandID = context.Brands
                            .Where(b => b.BrandName == th)
                            .Select(b => b.Brand_ID)
                            .FirstOrDefault();

                        var categoryID = context.Categories
                            .Where(c => c.CategoryName == dm)
                            .Select(c => c.Category_ID)
                            .FirstOrDefault();

                        if (string.IsNullOrEmpty(ma) || string.IsNullOrEmpty(ten) || gia <= 0 || idImg <= 0 || string.IsNullOrEmpty(dm) || string.IsNullOrEmpty(th))
                        {
                            throw new ArgumentException("Vui lòng nhập đầy đủ và chính xác thông tin sản phẩm.");
                        }

                        var product = context.Products.FirstOrDefault(p => p.Product_ID == ma);
                        if (product != null)
                        {
                            product.ProductName = ten;
                            product.UnitPrice = gia;
                            product.Quantity = sl;
                            product.Brand_ID = brandID;
                            product.Category_ID = categoryID;
                            product.Picture_ID = idImg;

                            context.SaveChanges();
                            dbContextTransaction.Commit();
                            return true; // Trả về true nếu cập nhật thành công
                        }
                        else
                        {
                        
                            return false; // Trả về false nếu không tìm thấy sản phẩm
                        }
                    }
                    catch (Exception ex)
                    {
                        
                        dbContextTransaction.Rollback();
                        return false; // Trả về false nếu có lỗi xảy ra
                    }
                }
            }
        }

        // Method to add a new product
        public bool TaoSanPham(string ma, string ten, int gia, string th, string dm, int sl, string Img)
        {
            using (var context = new QLCuaHang())
            {
                try
                {
                    var brandID = context.Brands
                        .Where(b => b.BrandName.Contains(th))
                        .Select(b => b.Brand_ID)
                        .FirstOrDefault();

                    var categoryID = context.Categories
                        .Where(c => c.CategoryName.Contains(dm))
                        .Select(c => c.Category_ID)
                        .FirstOrDefault();

                    var picID = context.PictureProducts
                        .Where(p => p.Picture_Name.Contains(Img))
                        .Select(p => p.Picture_ID)
                        .FirstOrDefault();

                    if (string.IsNullOrEmpty(ma) || string.IsNullOrEmpty(ten) || gia <= 0 || string.IsNullOrEmpty(Img) || string.IsNullOrEmpty(th) || string.IsNullOrEmpty(dm))
                    {
                        throw new ArgumentException("Vui lòng nhập đầy đủ và chính xác thông tin sản phẩm.");
                    }

                    var product = new Product
                    {
                        Product_ID = ma,
                        ProductName = ten,
                        UnitPrice = gia,
                        Quantity = sl,
                        Brand_ID = brandID,
                        Category_ID = categoryID,
                        Picture_ID = picID
                    };

                    context.Products.Add(product);
                    context.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Lỗi khi thêm sản phẩm: " + ex.Message);
                    return false;
                }
            }
        }

    }
}
