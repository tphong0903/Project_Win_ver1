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
                bool f = dbkh.CapNhatKhachHang(ref err,
                    txtSDT.Text,
                    txtTen.Text,
                    txtNgaySinh.Value,
                    ComboGioiTinh.Text,
                    int.Parse(txtDiem.Text));
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

    }
}
