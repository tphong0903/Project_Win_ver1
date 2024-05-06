using BusinessAccessLayer;
using DataAccessLayer.Entities;
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
    public partial class KHDetail : Form
    {
        int Check;
        string Phone = null;
        DataTable dtKhachHang = null;
        DBKhachHang dbkh = null;
        public KHDetail(int check, string phone)
        {
            Check = check;
            Phone = phone;
            InitializeComponent();
            dbkh = new DBKhachHang();
            txtSDT.ReadOnly = true;
            txtTen.ReadOnly = true;
            txtDiem.ReadOnly = true;
            txtTotal.ReadOnly = true;
            SaveButton.Visible = false;
            if (Check == 2)
            {
                txtTen.ReadOnly = false;
                SaveButton.Visible = true;
            }
        }
        private void LoadData()
        {
            try
            {
                dgvSP.DataSource = dbkh.SPcuaKhachHang(Phone);

                dgvKhachHang.DataSource = dbkh.TimKhachHang(Phone, "");
                txtSDT.Text = dgvKhachHang.Rows[0].Cells[0].Value.ToString();
                txtTen.Text = dgvKhachHang.Rows[0].Cells[1].Value.ToString();
                txtNgaySinh.Text = dgvKhachHang.Rows[0].Cells[2].Value.ToString();
                ComboGioiTinh.Text = dgvKhachHang.Rows[0].Cells[3].Value.ToString();
                txtDiem.Text = dgvKhachHang.Rows[0].Cells[4].Value.ToString();
                string a = (string.IsNullOrEmpty(dgvKhachHang.Rows[0].Cells[5].Value.ToString()) ? "0" : dgvKhachHang.Rows[0].Cells[5].Value.ToString());
                decimal value = Convert.ToDecimal(a);
                txtTotal.Text = value.ToString("N0"); 

            }
            catch (SqlException x)
            {
                MessageBox.Show(x.ToString());
            }
        }
        private void Form_load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void AddButton_Click_1(object sender, EventArgs e)
        {
            string err = "";
            try
            {
                // Hiển thị hộp thoại xác nhận
                DialogResult result = MessageBox.Show("Bạn có chắc muốn cập nhật thông tin khách hàng này không?", "Xác nhận cập nhật", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes) // Nếu người dùng chọn "Có"
                {
                    bool f = dbkh.CapNhatKhachHang(ref err,
                                                    txtSDT.Text,
                                                    txtTen.Text,
                                                    txtNgaySinh.Value,
                                                    ComboGioiTinh.Text,
                                                    int.Parse(txtDiem.Text));
                    if (f)
                    {
                        LoadData(); // Tải lại dữ liệu sau khi cập nhật thành công
                        MessageBox.Show("Đã cập nhật thông tin khách hàng thành công!");
                    }
                    else
                    {
                        MessageBox.Show("Cập nhật thông tin khách hàng không thành công!\n\rLỗi: " + err);
                    }
                }
                else
                {
                    // Người dùng chọn "Không" hoặc đóng hộp thoại xác nhận
                    MessageBox.Show("Thao tác cập nhật đã bị hủy bởi người dùng.");
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Điểm phải là một số nguyên hợp lệ!");
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Không thể cập nhật thông tin khách hàng. Lỗi SQL: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật thông tin khách hàng: " + ex.Message);
            }
        }

    }
}
