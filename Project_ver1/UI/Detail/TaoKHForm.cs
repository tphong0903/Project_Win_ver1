using BusinessAccessLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Project_ver1.UI.Detail
{
    public partial class TaoKHForm : Form
    {
        DBKhachHang dbKHang;
        public TaoKHForm()
        {
            InitializeComponent();
            dbKHang = new DBKhachHang();

        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            string err = "";
            try
            {
                // Hiển thị hộp thoại xác nhận
                DialogResult result = MessageBox.Show("Bạn có chắc muốn thêm thông tin khách hàng này không?", "Xác nhận thêm khách hàng", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes) // Nếu người dùng chọn "Có"
                {
                    // Thực hiện thêm thông tin khách hàng vào cơ sở dữ liệu
                    bool f = dbKHang.ThemKhachHang(ref err,
                                                    txtSdt.Text,
                                                    txtName.Text,
                                                    dtpBirthday.Value,
                                                    ComboGT.Text,
                                                    int.Parse(txtPoint.Text));
                    if (f)
                    {
                        MessageBox.Show("Đã thêm thông tin khách hàng thành công!");
                    }
                    else
                    {
                        MessageBox.Show("Không thêm được thông tin khách hàng!\n\rLỗi: " + err);
                    }
                }
                else
                {
                    // Người dùng chọn "Không" hoặc đóng hộp thoại xác nhận
                    MessageBox.Show("Thao tác thêm thông tin khách hàng đã bị hủy bởi người dùng.");
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Vui lòng kiểm tra định dạng đầu vào!");
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Không thêm được thông tin khách hàng. Lỗi SQL: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi khi thêm thông tin khách hàng: " + ex.Message);
            }
        }
    }
}
