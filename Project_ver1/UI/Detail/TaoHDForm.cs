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
    public partial class TaoHDForm : Form
    {
        DBSanPham dbsp= null;
        DBHoaDon dbhd;
        string hd;
        int r=0;
        int x = 0;
        int Tong = 0;
        double PhanTram;
        public TaoHDForm(string s)
        {
            InitializeComponent();
            dbsp= new DBSanPham();
            dbhd = new DBHoaDon();
            txtMaNV.Text = s;
        }
     
        private void LoadData()
        {
            try
            {
                dgvSanPham.DataSource = dbsp.LaySanPham();
                dgv.DataSource = dbhd.LayHoaDon();
                int s = dgv.RowCount+1;
                string hd = "HD";
                if (s < 10)
                    hd = hd + "0000";
                else if(s<100)
                    hd = hd + "000";
                else if (s < 1000)
                    hd = hd + "00";
                else if (s < 10000)
                    hd = hd + "0";
                txtMaHD.Text= hd+s;
                txtMaHD.ReadOnly= true;

            }
            catch (SqlException ex)
            {
                MessageBox.Show("Không lấy được nội dung trong.Lỗi rồi!!! \n\n Lỗi: "+ ex.Message);
            }
        }
        #region Event
        //nut Them San Pham
        private void ThemSPButton_Click(object sender, EventArgs e)
        {
            r = dgvSanPham.CurrentCell.RowIndex;
            string MaSP = dgvSanPham.Rows[r].Cells[0].Value.ToString();
            string TenSP = dgvSanPham.Rows[r].Cells[1].Value.ToString();
            string GiaBan = dgvSanPham.Rows[r].Cells[2].Value.ToString();
            string SLCon = dgvSanPham.Rows[r].Cells[3].Value.ToString();
            string SL = SLmua.Text;
            PhanTram = dbhd.LayGiamGia(txtGiamGia.Text);
            if (SL == "" || Int32.Parse(SL) > Int32.Parse(SLCon))
            {
                MessageBox.Show("Khong du san pham");
            }
            else
            {
                Tong = Tong + Int32.Parse(SL) * Int32.Parse(GiaBan);
                txtTongTien.Text = (Tong*(1- PhanTram/100)).ToString();
                dgvSPMua.Rows.Add(new Object[] { MaSP, TenSP, GiaBan, SL });
                dgvSanPham.Rows[r].Cells[3].Value = (Int32.Parse(SLCon) - Int32.Parse(SL)).ToString();
            }
        }
        private void TaoHDForm_Load(object sender, EventArgs e)
        {
            LoadData();
        }
        private void FindButton_Click(object sender, EventArgs e)
        {
            try
            {
               
                Name = NameText.Text.ToLower();
                dgvSanPham.DataSource = dbsp.FindSanPham(Name, "", "");
            

            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Xoa_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra xem dgvSPMua có khác null không
                if (dgvSPMua != null)
                {
                    // Lấy chỉ số (index) của dòng hiện tại đang được chọn
                    int rowIndex = dgvSPMua.CurrentCell?.RowIndex ?? -1;

                    // Kiểm tra nếu có dòng nào đang được chọn
                    if (rowIndex >= 0 && rowIndex < dgvSPMua.Rows.Count)
                    {
                        Tong = Tong - Int32.Parse(dgvSPMua.Rows[rowIndex].Cells[2].Value.ToString()) * Int32.Parse(dgvSPMua.Rows[rowIndex].Cells[3].Value.ToString());
                        txtTongTien.Text = (Tong * (1 - PhanTram / 100)).ToString();
                        dgvSPMua.Rows.RemoveAt(rowIndex);
                        
                    }
                    else
                    {
                        // Hiển thị thông báo nếu không có dòng nào được chọn để xóa
                        MessageBox.Show("Vui lòng chọn dòng cần xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    // Hiển thị thông báo nếu dgvSPMua là null
                    MessageBox.Show("DataGridView chưa được khởi tạo.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ nếu có lỗi xảy ra trong quá trình xóa dòng
                MessageBox.Show("Đã xảy ra lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Luu_Click(object sender, EventArgs e)
        {
            string err = "";
            try
            {
                // Hiển thị hộp thoại xác nhận
                DialogResult result = MessageBox.Show("Bạn có chắc muốn thêm hóa đơn này không?", "Xác nhận thêm hóa đơn", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes) // Nếu người dùng chọn "Có"
                {
                    bool f = dbhd.ThemHoaDon(ref err, txtMaHD.Text, txtSDT.Text, txtMaNV.Text, txtDate.Value, 0, txtGiamGia.Text);
                    if (f)
                    {
                        foreach (DataGridViewRow row in dgvSPMua.Rows)
                        {
                            if (row.Cells[0].Value != null)
                            {
                                string maSP = row.Cells[0].Value.ToString();
                                int soLuong = Convert.ToInt32(row.Cells[3].Value);
                                bool success = dbhd.ThemChiTietHoaDon(ref err, txtMaHD.Text, maSP, soLuong);
                            }
                        }
                        MessageBox.Show("Thêm hóa đơn thành công!");
                    }
                    else
                    {
                        MessageBox.Show("Thêm hóa đơn thất bại!\n\rLỗi: " + err);
                    }
                }
                else
                {
                    // Người dùng chọn "Không" hoặc đóng hộp thoại xác nhận
                    MessageBox.Show("Thao tác đã bị hủy bởi người dùng.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm hóa đơn: " + ex.Message);
            }
        }
        #endregion
    }

}
