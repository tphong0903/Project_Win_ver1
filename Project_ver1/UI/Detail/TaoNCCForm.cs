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

namespace Project_ver1.UI.Detail
{
    public partial class TaoNCCForm : Form
    {
        DBNhaCungCap dbncc = null;
        public TaoNCCForm()
        {
            InitializeComponent();
            dbncc = new DBNhaCungCap();
        }
        private void SaveButton_Click(object sender, EventArgs e)
        {
            string err = "";
            try
            {
                // Hiển thị hộp thoại xác nhận
                DialogResult result = MessageBox.Show("Bạn có chắc muốn thêm thông tin nhà cung cấp này không?", "Xác nhận thêm nhà cung cấp", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes) // Nếu người dùng chọn "Có"
                {
                    // Thực hiện thêm thông tin nhà cung cấp vào cơ sở dữ liệu
                    bool f = dbncc.ThemNhaCungCap(ref err,
                                                    textBoxMaNhaCungCap.Text,
                                                    textBoxTenNhaCungCap.Text,
                                                    textBoxSoDienThoai.Text,
                                                    textBoxDiaChi.Text,
                                                    textBoxEmail.Text);
                    if (f)
                    {
                        MessageBox.Show("Đã thêm thông tin nhà cung cấp thành công!");
                    }
                    else
                    {
                        MessageBox.Show("Không thêm được thông tin nhà cung cấp!\n\rLỗi: " + err);
                    }
                }
                else
                {
                    // Người dùng chọn "Không" hoặc đóng hộp thoại xác nhận
                    MessageBox.Show("Thao tác thêm thông tin nhà cung cấp đã bị hủy bởi người dùng.");
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Không thể truy cập cơ sở dữ liệu!!!\n\nLỗi: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi khi thêm thông tin nhà cung cấp: " + ex.Message);
            }
        }

    }
}
