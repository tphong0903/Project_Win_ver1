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
    public partial class NCCDetail : Form
    {
        DBNhaCungCap DBNhaCungCap = null;
        int Check;
        string ID = null;
        public NCCDetail(int check, string iD)
        {
            Check = check;
            ID = iD;
            InitializeComponent();
            DBNhaCungCap = new DBNhaCungCap();
            if(Check==1) 
            {
                SaveButton.Visible=false;
            }
        }
        private void LoadData()
        {
            try
            {
                dgvSanPham.DataSource = DBNhaCungCap.SPCuaNhaCungCap(ID);
                dgv.DataSource = DBNhaCungCap.TimNhaCungCap(ID, "");
                textBoxMaNhaCungCap.Text = dgv.Rows[0].Cells[0].Value.ToString();
                textBoxTenNhaCungCap.Text = dgv.Rows[0].Cells[1].Value.ToString();
                textBoxSoDienThoai.Text = dgv.Rows[0].Cells[2].Value.ToString();
                textBoxDiaChi.Text = dgv.Rows[0].Cells[3].Value.ToString();
                textBoxEmail.Text= dgv.Rows[0].Cells[4].Value.ToString();
                Tong.Text= dgv.Rows[0].Cells[5].Value.ToString() == "" ? "0": dgv.Rows[0].Cells[5].Value.ToString().ToString();
            }
            catch (SqlException ex)
            {
                this.Close();
                MessageBox.Show("Không thể truy cập!!!\n\nLỗi: " + ex.Message);
            }
        }

        private void NCCDetail_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            string err = "";
            try
            {
                // Lệnh Update
                bool f = DBNhaCungCap.CapNhatNhaCungCap(ref err,
                this.textBoxMaNhaCungCap.Text.ToString(),
                this.textBoxTenNhaCungCap.Text.ToString(),
                this.textBoxSoDienThoai.Text.ToString(),
                this.textBoxDiaChi.Text.ToString(),
                this.textBoxEmail.Text.ToString());
                if (f)
                {
                    MessageBox.Show("Đã cập nhật xong!");
                }
                else
                {
                    MessageBox.Show("Đã cập nhật chưa xong!\n\r" + "Lỗi:" + err);
                }
            }
            catch (SqlException ex)
            {
                this.Close();
                MessageBox.Show("Không thể truy cập!!!\n\nLỗi: " + ex.Message);
            }
        }
    }

}
