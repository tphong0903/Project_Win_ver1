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
    public partial class BLDetail : Form
    {
        int Check;
        string ID;
        DBBienLai dbbl = null;
        public BLDetail(int check,string id)
        {
            Check = check;
            ID = id;
            InitializeComponent();
            dbbl = new DBBienLai();
        }
        public void LoadData()
        {
            try
            {

                dgvSanPham.DataSource = dbbl.SPCuaBienLai(ID);
                dgv.DataSource = dbbl.TimBienLai(ID, "");
                textBoxMaBienLai.Text = dgv.Rows[0].Cells[0].Value.ToString();
                textBoxMaNhaCungCap.Text = dgv.Rows[0].Cells[3].Value.ToString();
                textBoxTenNhaCungCap.Text = dgv.Rows[0].Cells[4].Value.ToString();
                dateTimePickerNgayThanhToan.Text = dgv.Rows[0].Cells[1].Value.ToString();
                textBoxThanhTien.Text = dgv.Rows[0].Cells[2].Value.ToString();
            }
            catch (SqlException ex)
            {
                this.Close();
                MessageBox.Show("Không thể truy cập!!!\n\nLỗi: " + ex.Message);
            }
        }

        private void BLDetail_Load(object sender, EventArgs e)
        {
            LoadData();
        }
    }
}
