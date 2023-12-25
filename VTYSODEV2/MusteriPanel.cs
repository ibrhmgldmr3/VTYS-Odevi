using Npgsql;
using ReaLTaiizor.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VTYSODEV2
{
    public partial class MusteriPanel : Form
    {
        string baglanti = "Server=localhost;Port=5432;Database=Banka2;User Id=postgres;Password=0326;";
        
        long kisiid = 0;

        public MusteriPanel(string kisiId)
        {
            InitializeComponent();
            kisiid = Convert.ToInt64(kisiId);
            DovizGizleme();

        }
        private void DovizGizleme()
        {
            miktarText.Visible = false;
            dungeonLabel1.Visible = false;
            dungeonLabel2.Visible = false;
            hesapNo.Visible = false;
            dungeonLabel3.Visible = false;
            skyTextBox1.Visible = false;
            airButton6.Visible = false;
            skyTextBox2.Visible = false;
            airButton5.Visible = false;
            airButton7.Visible = false;
            dungeonLabel4.Visible = false;
        }
        private void airButton2_Click(object sender, EventArgs e)
        {
            try 
            { 
                using (var conn = new NpgsqlConnection(baglanti))
                {
                    conn.Open();

                    string query = string.Empty;

                    if (airRadioButton1.Checked)
                    {
                    
                        query = "SELECT * FROM Hesap INNER JOIN DovizHesap ON Hesap.HesapId = DovizHesap.DovizHesapID WHERE Hesap.MusteriId = @musteriId";
                    }
                    else if (airRadioButton2.Checked)
                    { 
                        query = "SELECT * FROM Hesap INNER JOIN VadesizHesap ON Hesap.HesapId = VadesizHesap.HesapId WHERE Hesap.MusteriId = @musteriId";
                    }
                    else if (airRadioButton3.Checked)
                    {

                        query = "SELECT * FROM Hesap INNER JOIN VadeliHesap ON Hesap.HesapId = VadeliHesap.HesapId WHERE Hesap.MusteriId = @musteriId";
                    }

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@musteriId", kisiid);

                        using (var dataAdapter = new NpgsqlDataAdapter(cmd))
                        {
                            var ds = new DataSet();
                            dataAdapter.Fill(ds);

                            dataGridView2.DataSource = ds.Tables[0];
                        }
                    }
                }
            }  
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

        private void dataGridView2_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView2.SelectedRows[0];
                hesapNo.Text = selectedRow.Cells["hesapid"].Value.ToString();
            }

        }

        private void airButton5_Click(object sender, EventArgs e)
        {
            using (var conn = new NpgsqlConnection(baglanti))
            {
                try
                {
                    conn.Open();

                    using (var cmd = new NpgsqlCommand("call doviz_alim(@doviz_hesap_id, @alim_miktar)", conn))
                    {
                        // Parametreleri ekleyerek fonksiyona değer geçirebilirsiniz.
                        cmd.Parameters.AddWithValue("doviz_hesap_id", Convert.ToInt32(hesapNo.Text));
                        cmd.Parameters.AddWithValue("alim_miktar", Convert.ToDecimal(miktarText.Text));

                        // Fonksiyon sonuçlarını okuma
                        var result = cmd.ExecuteScalar();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                }
            }
        }

        private void airButton6_Click(object sender, EventArgs e)
        {
            using (var conn = new NpgsqlConnection(baglanti))
            {
                try
                {
                    conn.Open();

                    using (var cmd = new NpgsqlCommand("SELECT doviz_hesap_degeri(@doviz_hesap_id)", conn))
                    {
                        // Parametreleri ekleyerek fonksiyona değer geçirebilirsiniz.
                        cmd.Parameters.AddWithValue("doviz_hesap_id", Convert.ToInt32(hesapNo.Text));
                        // Fonksiyon sonuçlarını okuma
                        var result = cmd.ExecuteScalar();
                        skyTextBox1.Text = Convert.ToString(result);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                }
            }
        }

        private void airRadioButton1_CheckedChanged(object sender)
        {
            miktarText.Visible = true;
            dungeonLabel1.Visible = true;
            dungeonLabel2.Visible = true;
            hesapNo.Visible = true;
            dungeonLabel3.Visible = true;
            skyTextBox1.Visible = true;
            airButton6.Visible = true;
            skyTextBox2.Visible = true;
            airButton5.Visible = true;
            airButton7.Visible = true;
            dungeonLabel4.Visible = true;
            hesapNo.Text = "";
        }

        private void airRadioButton2_CheckedChanged(object sender)
        {
            DovizGizleme();
        }

        private void airRadioButton3_CheckedChanged(object sender)
        {
            DovizGizleme();
        }

        private void airButton7_Click(object sender, EventArgs e)
        {
            using (var conn = new NpgsqlConnection(baglanti))
            {
                try
                {
                    conn.Open();

                    using (var cmd = new NpgsqlCommand("call doviz_satim(@doviz_hesap_id, @satim_miktar)", conn))
                    {
                        // Parametreleri ekleyerek fonksiyona değer geçirebilirsiniz.
                        cmd.Parameters.AddWithValue("doviz_hesap_id", Convert.ToInt32(hesapNo.Text));
                        cmd.Parameters.AddWithValue("satim_miktar", Convert.ToDecimal(skyTextBox2.Text));

                        // Fonksiyon sonuçlarını okuma
                        var result = cmd.ExecuteScalar();
                        miktarText.Text = Convert.ToString(result);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                }
            }
        }

        private void airButton8_Click(object sender, EventArgs e)
        {
            using (var conn = new NpgsqlConnection(baglanti))
            {
                try
                {
                    conn.Open();

                    using (var cmd = new NpgsqlCommand("call para_cek(@doviz_hesap_id, @cekilen_miktar)", conn))
                    {
                        // Parametreleri ekleyerek fonksiyona değer geçirebilirsiniz.
                        cmd.Parameters.AddWithValue("doviz_hesap_id", Convert.ToInt32(hesapNo.Text));
                        cmd.Parameters.AddWithValue("cekilen_miktar", Convert.ToDecimal(skyTextBox3.Text));

                        // Fonksiyon sonuçlarını okuma
                        var result = cmd.ExecuteScalar();
                        miktarText.Text = Convert.ToString(result);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                }
            }
            airButton2_Click(sender, e);

        }

        private void airButton9_Click(object sender, EventArgs e)
        {
                try
                {
            using (var conn = new NpgsqlConnection(baglanti))
            {
                
                    conn.Open();

                    using (var cmd = new NpgsqlCommand("call para_yatir(@doviz_hesap_id, @yatirilan_miktar)", conn))
                    {
                        // Parametreleri ekleyerek fonksiyona değer geçirebilirsiniz.
                        cmd.Parameters.AddWithValue("doviz_hesap_id", Convert.ToInt32(hesapNo.Text));
                        cmd.Parameters.AddWithValue("yatirilan_miktar", Convert.ToDecimal(skyTextBox4.Text));
                        // Fonksiyon sonuçlarını okuma
                        var result = cmd.ExecuteScalar();
                    }
                }
            
            }    catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                }
            airButton2_Click(sender, e);

        }

        private void airButton1_Click(object sender, EventArgs e)
        {
            try { 
            using (var conn = new NpgsqlConnection(baglanti))
            {
                conn.Open();

                string query = "SELECT * FROM Hesap where musteriid=@musteriid";
                ;
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@musteriid",Convert.ToInt32(aloneTextBox2.Text));

                    using (var dataAdapter = new NpgsqlDataAdapter(cmd))
                    {
                        var ds = new DataSet();
                        dataAdapter.Fill(ds);

                        dataGridView3.DataSource = ds.Tables[0];
                    }
                }
            }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }


        private void airButton3_Click(object sender, EventArgs e)
        {
            using (var conn = new NpgsqlConnection(baglanti))
            {
                try
                {
                    conn.Open();
                    DataGridViewRow selectedRow = dataGridView3.SelectedRows[0];
                    string alici = selectedRow.Cells["hesapid"].Value.ToString();
                    using (var cmd = new NpgsqlCommand("call transfer_money(@_gonderen, @_alici,@_miktar)", conn)) 
                    {
                        // Parametreleri ekleyerek fonksiyona değer geçirebilirsiniz.
                        cmd.Parameters.AddWithValue("_gonderen", Convert.ToInt32(hesapNo.Text));
                        cmd.Parameters.AddWithValue("_alici", Convert.ToInt32(alici));
                        cmd.Parameters.AddWithValue("_miktar", Convert.ToDecimal(dreamTextBox9.Text));

                        // Fonksiyon sonuçlarını okuma
                        var result = cmd.ExecuteScalar();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                }
            }
        }

        private void airButton2_Click_1(object sender, EventArgs e)
        {
            airButton2_Click(sender, e);
        }

        private void airButton8_Click_1(object sender, EventArgs e)
        {
            airButton8_Click(sender, e);
        }

        private void airButton9_Click_1(object sender, EventArgs e)
        {
            airButton9_Click(sender, e);
        }

        private void airButton1_Click_1(object sender, EventArgs e)
        {
            airButton1_Click(sender, e);
        }

        private void airButton4_Click(object sender, EventArgs e)
        {
            if(dataGridView2.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen bir hesap seçiniz.");
                return;
            }
            DataGridViewRow selectedRow = dataGridView2.SelectedRows[0];
            int selectedRowIndex = dataGridView2.SelectedCells[0].RowIndex;

            // Kontrol etmek istediğiniz sütunun index'ini al
            int columnIndex = dataGridView2.Columns["hesapid"].Index; // "ColumnName" yerine kendi sütun adınızı yazın

            // Seçilen hücrenin değerini al
            object cellValue = dataGridView2.Rows[selectedRowIndex].Cells[columnIndex].Value;


            // Değeri kontrol et
            if (cellValue == null || cellValue == DBNull.Value)
            {
                MessageBox.Show("Seçilen hücre boş (null)!");
                return;
            }

            // Seçilen satırın verilerine erişim örnek:
            if (Convert.ToInt32(selectedRow.Cells["hesapid"].Value) != 0)
            {
                HesapIslemleri hesapIslemleri = new HesapIslemleri(Convert.ToInt32(selectedRow.Cells["hesapid"].Value));
                hesapIslemleri.Show();
            }

        }
    }
}
