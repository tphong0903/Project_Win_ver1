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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace Project_ver1.UI.Detail
{
    public partial class HDDetail : Form
    {
        int Check;
        string ID;
        DBHoaDon dbhd=null;
        public HDDetail(int check , string id)
        {
            Check = check;
            ID = id;
            InitializeComponent();
            dbhd = new DBHoaDon();
        }
        public void LoadData()
        {
            try
            {
                dgvSP.DataSource = dbhd.SPCuaHoaDon(ID);

                dgv.DataSource = dbhd.TimHoaDon(ID, "");
                MaHD.Text = dgv.Rows[0].Cells[0].Value.ToString();
                SoDienThoai.Text = dgv.Rows[0].Cells[6].Value.ToString();
                TenKH.Text = dgv.Rows[0].Cells[1].Value.ToString();
                Ngay.Text = dgv.Rows[0].Cells[3].Value.ToString();
                ThanhTien.Text = dgv.Rows[0].Cells[4].Value.ToString();
                GiamGia.Text = dgv.Rows[0].Cells[5].Value.ToString();
                TenNV.Text = dgv.Rows[0].Cells[2].Value.ToString();
            }
            catch(SqlException ex) 
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void HDDetail_Load(object sender, EventArgs e)
        {
            LoadData();
        }
    }
}
