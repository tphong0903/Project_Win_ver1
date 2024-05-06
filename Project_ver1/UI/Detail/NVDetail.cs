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

namespace Project_ver1.UI
{
    public partial class NVDetail : Form
    {
        int Check;
        string ID;
        DBNhanVien dbnv = null;
        public NVDetail(int check, string iD)
        {
            Check = check;
            ID = iD.ToUpper();
            InitializeComponent();
            dbnv = new DBNhanVien();
            SaveButton.Visible = false;
            if(Check ==1)
                LoadData();
            else if (Check ==2)
            {
                SaveButton.Visible = true;
                this.Text = "Cập nhật thông tin nhân viên";
                LoadData();
            }   
            else 
            {
                SaveButton.Visible = true;
                textBoxTrangThai.Text = "1";
                textBoxTrangThai.Enabled = false;
                textBoxTotal.Visible = false;
                gunaLabel4.Visible = false;
                this.Text = "Thêm nhân viên mới";
            }

        }
        private void LoadData()
        {
            try
            {
                dgv.DataSource = dbnv.TimAllNhanVien(ID);
                textBoxMaNV.Text= dgv.Rows[0].Cells[0].Value.ToString();
                textBoxTenNV.Text = dgv.Rows[0].Cells[1].Value.ToString();
                dateTimePicker.Text = dgv.Rows[0].Cells[2].Value.ToString();
                comboBoxGioiTinh.Text = dgv.Rows[0].Cells[3].Value.ToString();
                textBoxDiaChi.Text = dgv.Rows[0].Cells[4].Value.ToString();
                textBoxSDT.Text = dgv.Rows[0].Cells[5].Value.ToString();
                comboBoxChucVu.Text = dgv.Rows[0].Cells[6].Value.ToString();
                textBoxTrangThai.Text = dgv.Rows[0].Cells[7].Value.ToString();
                textBoxMK.Text = dgv.Rows[0].Cells[8].Value.ToString();
                textBoxTotal.Text = dgv.Rows[0].Cells[9].Value.ToString();
            }
            catch (SqlException x)
            {
                this.Close();
                MessageBox.Show(" Lỗi rồi! \n"+x.Message);
            }
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            string err = "";
            try
            {
                // Hiển thị hộp thoại xác nhận
                DialogResult result = MessageBox.Show("Bạn có chắc muốn thực hiện hành động này không?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes) // Nếu người dùng chọn "Có"
                {
                    if (Check == 2) // Nếu đang cập nhật thông tin nhân viên
                    {
                        bool f = dbnv.CapNhatNhanVien(ref err,
                                                        textBoxMaNV.Text,
                                                        textBoxTenNV.Text,
                                                        dateTimePicker.Value,
                                                        comboBoxGioiTinh.Text,
                                                        textBoxDiaChi.Text,
                                                        textBoxSDT.Text,
                                                        comboBoxChucVu.Text,
                                                        int.Parse(textBoxTrangThai.Text),
                                                        textBoxMK.Text);
                        if (f)
                        {
                            MessageBox.Show("Đã cập nhật thông tin nhân viên thành công!");
                        }
                        else
                        {
                            MessageBox.Show("Cập nhật thông tin nhân viên không thành công!\n\rLỗi: " + err);
                        }
                    }
                    else if (Check == 3) // Nếu đang thêm mới nhân viên
                    {
                        bool f = dbnv.ThemNhanVien(ref err,
                                                    textBoxMaNV.Text,
                                                    textBoxTenNV.Text,
                                                    dateTimePicker.Value,
                                                    comboBoxGioiTinh.Text,
                                                    textBoxDiaChi.Text,
                                                    textBoxSDT.Text,
                                                    comboBoxChucVu.Text,
                                                    1, // Trạng thái mặc định khi thêm mới (ví dụ: 1 là hoạt động)
                                                    textBoxMK.Text);
                        if (f)
                        {
                            MessageBox.Show("Đã thêm nhân viên mới thành công!");
                        }
                        else
                        {
                            MessageBox.Show("Thêm mới nhân viên không thành công!\n\rLỗi: " + err);
                        }
                    }
                }
                else
                {
                    // Người dùng chọn "Không" hoặc đóng hộp thoại xác nhận
                    MessageBox.Show("Thao tác đã bị hủy bởi người dùng.");
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Vui lòng kiểm tra định dạng đầu vào!");
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Không thể truy cập cơ sở dữ liệu!!!\n\nLỗi: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi: " + ex.Message);
            }
        }
    }
}
