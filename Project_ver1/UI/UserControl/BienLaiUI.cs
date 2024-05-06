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
using Project_ver1.UI.Detail;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace Project_ver1.UI
{
    public partial class BienLaiUI : Form
    {
        DBBienLai dbbl;
        DataTable dtBienLai = null;
        BLDetail a = null;
        TaoBLForm b = null;
        string HD = null;
        string NCC = null;
        string hd = null;
        public BienLaiUI()
        {
            InitializeComponent();
            dbbl = new DBBienLai();
        }
        private void LoadData()
        {
            try
            {
                dgvBienLai.DataSource = dbbl.LayBienLai();

                HD = dgvBienLai.Rows[0].Cells[0].Value.ToString().ToLower();
                LabelSoBienLai.Text = (dgvBienLai.RowCount).ToString();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Không thể truy cập!!!\n\nLỗi: " + ex.Message);
            }
        }
        #region Event
        private void BienLaiUI_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void BienLaiUI_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void ReadButton_Click(object sender, EventArgs e)
        {
            a= new BLDetail(1,HD);
            a.ShowDialog();
        }

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            a = new BLDetail(2,HD);
            a.ShowDialog();
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            b = new TaoBLForm();
            b.ShowDialog();
        }
        private void dgvKhachHang_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int r = dgvBienLai.CurrentCell.RowIndex;
            HD = dgvBienLai.Rows[r].Cells[0].Value.ToString().ToLower();
            MaBienLai.Text = dgvBienLai  .Rows[r].Cells[0].Value.ToString();
            MaNhaCungCap.Text = dgvBienLai.Rows[r].Cells[3].Value.ToString();
            ngayNhap.Text = dgvBienLai.Rows[r].Cells[1].Value.ToString();
            TenNhaCungCap.Text = dgvBienLai.Rows[r].Cells[4].Value.ToString();
            decimal value = Convert.ToDecimal(dgvBienLai.Rows[r].Cells[2].Value);
            Total.Text = value.ToString("N0");

        }
        private void FindButton_Click(object sender, EventArgs e)
        {
            try
            {

                NCC = txtMNCC.Text;
                hd = MBL.Text;
                dgvBienLai.DataSource = dbbl.TimBienLai(hd, NCC);
                int r = dgvBienLai.RowCount;
                if (r > 1)
                {
                    HD = dgvBienLai.Rows[0].Cells[0].Value.ToString();
                    LabelSoBienLai.Text = (dgvBienLai.RowCount - 1).ToString();
                }

            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void ReloadButton_Click(object sender, EventArgs e)
        {
            LoadData();
        }
        #endregion
    }
}
