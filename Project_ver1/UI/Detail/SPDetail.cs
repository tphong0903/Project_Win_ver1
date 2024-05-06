using BusinessAccessLayer;
using DataAccessLayer.Entities;
using Project_ver1.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Project_ver1.UI
{
    public partial class SPDetail : Form
    {
        DBSanPham dbsp;
        DataTable dtSanPham = null;
        int Check;
        string Product_Id;
        Image img = null;
        string selectedFilePath = null;
        string maPic_ID = null;
        bool checkChangeImg = false;
        public SPDetail(int check, string Product_ID)
        {
            Check = check;
            Product_Id = Product_ID;

            InitializeComponent();
            textBoxMaSP.ReadOnly = true;
            textBoxTenSP.ReadOnly = true;
            textBoxGia.ReadOnly = true;
            SoLuong.ReadOnly = true;
            SaveButton.Visible = false;
            imgBtn.Visible = false;
            SaveButton.Visible = false;

            dbsp = new DBSanPham();

            ComboThuongHieu.DataSource = dbsp.LayThuongHieu();
            ComboThuongHieu.DisplayMember = "BrandName";

            ComboDanhMuc.DataSource = dbsp.LayDanhMuc();
            ComboDanhMuc.DisplayMember = "CategoryName";
            this.Text = "Chi tiết sản phẩm";

            if (Check == 1)
            {
                this.Text = "Chi tiết sản phẩm";
                textBoxMaSP.ReadOnly = false;
                textBoxTenSP.ReadOnly = false;
                textBoxGia.ReadOnly = false;
                imgBtn.Visible = true;
                SaveButton.Visible = true;
            }
            else if (Check == 2)
            {
                this.Text = "Thêm sản phẩm";
                textBoxMaSP.ReadOnly = false;
                textBoxTenSP.ReadOnly = false;
                textBoxGia.ReadOnly = false;
                SoLuong.ReadOnly = false;
                SaveButton.Visible = true;
                textBoxGia.ReadOnly = false;
                imgBtn.Visible = true;
            }
            

        }
        public void LoadData()
        {
            try
            {
                List<Product> productList = dbsp.ChiTietSP(Product_Id);
                foreach (var a in productList)
                {
                    textBoxMaSP.Text = a.Product_ID.ToString();
                    textBoxTenSP.Text = a.ProductName;
                    textBoxGia.Text = a.UnitPrice.ToString();
                    SoLuong.Text = a.Quantity.ToString();
                    ComboThuongHieu.Text = a.Brand.BrandName.ToString();
                    ComboDanhMuc.Text = a.Category.CategoryName.ToString(); // If you include CategoryName in the query projection
                    PictureProduct.Image = GetImageByName(a.PictureProduct.Picture_Name.ToString());
                    maPic_ID = a.PictureProduct.Picture_ID.ToString();
                    img = PictureProduct.Image;
                }
               
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private Image GetImageByName(string imageName)
        {
            // Đường dẫn tới thư mục IMG trong thư mục Image của project_ver1
            string imgFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../Project_ver1/UI/Image");
            try
            {
                // Đường dẫn đến tệp tin ảnh bằng tên hình ảnh
                string imgFilePath = Path.Combine(imgFolderPath, imageName);
                // Kiểm tra xem tệp tin ảnh có tồn tại không
                if (File.Exists(imgFilePath))
                {
                    // Tạo một đối tượng Image từ tệp tin ảnh
                    Image image = Image.FromFile(imgFilePath);
                    return image;
                }
                else
                {
                    MessageBox.Show("Không tìm thấy hình ảnh có tên: " + imageName);
                    return null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
                return null;
            }
        }
        #region Event
        private void DetailForm_Load(object sender, EventArgs e)
        {
            if (Product_Id == "")
            {
                MessageBox.Show("Vui lòng chọn một hàng");
                this.Close();
            }
            else if (Check == 1 || Check == 0)
                LoadData();
            
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            bool f = false;
            string err = "";

            try
            {
                // Hiển thị hộp thoại xác nhận
                DialogResult result = MessageBox.Show("Bạn có chắc muốn lưu các thay đổi này không?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes) // Nếu người dùng chọn "Có"
                {
                    if (Check == 2 && img == null)
                    {
                        MessageBox.Show("Vui lòng chọn một hình ảnh trước khi lưu!");
                        return;
                    }

                    if (checkChangeImg)
                    {
                        string imgFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../Project_ver1/UI/Image");
                        string imgFileName = Path.GetFileName(selectedFilePath);
                        string imgFilePath = Path.Combine(imgFolderPath, imgFileName);

                        if (Check == 2)
                        {
                            // Lưu hình ảnh mới vào thư mục ứng dụng
                            img.Save(imgFilePath);
                            // Thêm hình ảnh vào cơ sở dữ liệu
                            f = dbsp.ThemHinhAnh(imgFileName);
                        }
                        else if (Check == 1)
                        {
                            string present = "";
                            List<Product> productList = dbsp.ChiTietSP(Product_Id);
                            foreach (var product in productList)
                            {
                                present = product.PictureProduct.Picture_Name.ToString();
                            }

                            // Nếu hình ảnh mới khác với hình ảnh hiện tại của sản phẩm, thực hiện cập nhật hình ảnh
                            if (present != imgFileName)
                            {
                                // Lưu hình ảnh mới vào thư mục ứng dụng
                                img.Save(imgFilePath);
                                // Cập nhật hình ảnh trong cơ sở dữ liệu
                                f = dbsp.SuaHinhAnh(imgFileName, int.Parse(maPic_ID));
                            }
                        }

                        if (f)
                        {
                            // Nếu thao tác thêm hoặc cập nhật hình ảnh thành công, tiến hành cập nhật hoặc thêm mới sản phẩm
                            UpateOrAddProduct(imgFileName);
                        }
                        else
                        {
                            MessageBox.Show("Đã xảy ra lỗi khi lưu hình ảnh vào cơ sở dữ liệu!");
                        }
                    }
                    else
                    {
                        // Nếu không có thay đổi hình ảnh, tiến hành cập nhật hoặc thêm mới sản phẩm với hình ảnh trống
                        UpateOrAddProduct("");
                    }
                }
                else
                {
                    // Người dùng chọn "Không" hoặc đóng hộp thoại xác nhận, không thực hiện thay đổi
                    MessageBox.Show("Thao tác đã bị hủy bởi người dùng.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi: " + ex.Message);
            }
        }

        private void UpateOrAddProduct(string imgFileName)
        {
            string err = "";

            if (Check == 1)
            {
                try
                {
                    bool f = dbsp.CapNhatSanPham(
                        textBoxMaSP.Text,
                        textBoxTenSP.Text,
                        int.Parse(textBoxGia.Text),
                        ComboThuongHieu.Text,
                        ComboDanhMuc.Text,
                        int.Parse(SoLuong.Text),
                        int.Parse(maPic_ID));
                    if (f)
                    {
                        LoadData();
                        MessageBox.Show("Đã cập nhật xong!");
                    }
                    else
                    {
                        MessageBox.Show("Đã cập nhật chưa xong!\n\r" + "Lỗi:" + err);
                    }
                }
                catch (SqlException)
                {
                    MessageBox.Show("Không cập nhật được. Lỗi rồi!");
                }
            }
            else
            {
                try
                {
                    bool f;
                    if (textBoxGia.Text!="")
                    {
                       f = dbsp.TaoSanPham(
                       textBoxMaSP.Text,
                       textBoxTenSP.Text,
                       int.Parse(textBoxGia.Text),
                       ComboThuongHieu.Text,
                       ComboDanhMuc.Text,
                       0,
                       imgFileName);
                       if (f)
                       {
                            LoadData();
                            MessageBox.Show("Đã thêm mới xong!");
                       }
                       else
                       {
                            MessageBox.Show(err);
                       }
                    }
                    else
                    {
                        MessageBox.Show("Vui lòng nhập giá!");
                    }
                    
                }
                catch (SqlException)
                {
                    MessageBox.Show("Không thêm mới được. Lỗi rồi!");
                }
            }
        }
        private void gunaButton1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Chọn Hình ảnh";
            openFileDialog.Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                selectedFilePath = openFileDialog.FileName;
                Image selectedImage = Image.FromFile(selectedFilePath);
                PictureProduct.Image = selectedImage;
                img = selectedImage;
                checkChangeImg = true;
            }
        }
        #endregion
        
    }
}
