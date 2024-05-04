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
                if (Check ==2)
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
                        MessageBox.Show("Đã cập nhật xong!");
                    }
                    else
                    {
                        MessageBox.Show("Đã cập nhật chưa xong!\n\r" + "Lỗi:" + err);
                    }
                } 
                else if (Check ==3)
                {
                    bool f = dbnv.ThemNhanVien(ref err,
                    textBoxMaNV.Text,
                    textBoxTenNV.Text,
                    dateTimePicker.Value,
                    comboBoxGioiTinh.Text,
                    textBoxDiaChi.Text,
                    textBoxSDT.Text,
                    comboBoxChucVu.Text,
                    1,
                    textBoxMK.Text);
                    if (f)
                    {
                        MessageBox.Show("Đã thêm xong!");
                    }
                    else
                    {
                        MessageBox.Show("Đã cập nhật chưa xong!\n\r" + "Lỗi:" + err);
                    }
                }
                //this.Close();
                
            }
            catch (SqlException)
            {
                MessageBox.Show("Không cập nhật được. Lỗi rồi!");
            }
        }
    }
}
