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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace VTYSODEV2
{
    public partial class YoneticiPanel : Form
    {
        string baglanti = "Server=localhost;Port=5432;Database=Banka2;User Id=postgres;Password=0326;";

        public YoneticiPanel()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
        private void spaceBorderLabel2_Click(object sender, EventArgs e)
        {

        }

        private void dungeonHeaderLabel6_Click(object sender, EventArgs e)
        {

        }

        private void dungeonHeaderLabel7_Click(object sender, EventArgs e)
        {

        }

        private void dungeonHeaderLabel8_Click(object sender, EventArgs e)
        {

        }

        private void dungeonHeaderLabel5_Click(object sender, EventArgs e)
        {

        }

        private void dreamTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void dreamTextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void dreamTextBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void dreamTextBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void airButton1_Click(object sender, EventArgs e)
        {
            string tckn = aloneTextBox2.Text; // TextBox'tan Doviz bilgisini alın, txtDoviz TextBox'ınızın adı olabilir

            if (string.IsNullOrWhiteSpace(tckn))
            {
                MessageBox.Show("TCKNO boş olamaz.");
                return;
            }

            using (NpgsqlConnection conn = new NpgsqlConnection(baglanti))
            {
                conn.Open();

                // Kullanıcı adı ve şifreyi kontrol etmek için sorgu
                string query = "SELECT * FROM public.\"kisi\" WHERE \"tckn\" = @tckn";
                NpgsqlCommand npgsqlCommand = new NpgsqlCommand(query, conn);
                npgsqlCommand.Parameters.AddWithValue("@tckn", tckn);

                NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(npgsqlCommand);
                DataSet ds = new DataSet();
                dataAdapter.Fill(ds);

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    dataGridView2.DataSource = ds.Tables[0];
                    MessageBox.Show("Kisi bulundu.");
                }
                else
                {
                    MessageBox.Show("Kisi bulunamadı.");
                }
            }
        }

        private void airButton2_Click(object sender, EventArgs e)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(baglanti))
            {      
                
                    conn.Open();
                    NpgsqlTransaction tran = conn.BeginTransaction();

                    try
                    {
                        // INSERT query
                        string query1 = "INSERT INTO public.\"kisi\" (\"ad\", \"soyad\", \"tckn\", \"dogumtarihi\",\"cinsiyet\",\"telefon\",\"sifre\",\"kisituru\") VALUES (@ad, @soyad, @tckn, @dogumtarihi,@cinsiyet,@telefon,@sifre,@kisituru) RETURNING \"kisiid\"";
                        NpgsqlCommand npgsqlCommand = new NpgsqlCommand(query1, conn);
                        npgsqlCommand.Transaction = tran;

                        // Add parameters
                        npgsqlCommand.Parameters.AddWithValue("@ad", dreamTextBox4.Text);
                        npgsqlCommand.Parameters.AddWithValue("@soyad", dreamTextBox10.Text);
                        npgsqlCommand.Parameters.AddWithValue("@tckn", dreamTextBox13.Text);
                        npgsqlCommand.Parameters.AddWithValue("@dogumtarihi", dateTimePicker1.Value.Date);
                        npgsqlCommand.Parameters.AddWithValue("@cinsiyet", Convert.ToChar(dreamTextBox11.Text));
                        npgsqlCommand.Parameters.AddWithValue("@telefon", dreamTextBox3.Text);
                        npgsqlCommand.Parameters.AddWithValue("@sifre", dreamTextBox12.Text);
                        npgsqlCommand.Parameters.AddWithValue("@kisituru", dreamTextBox2.Text);

                        int kisiId = (int)npgsqlCommand.ExecuteScalar();

                        // Insert into Musteri table
                        string insertMusteriQuery = "INSERT INTO Musteri (kisiid, musterituru) VALUES (@kisiid, @musterituru)";
                        using (NpgsqlCommand cmdMusteri = new NpgsqlCommand(insertMusteriQuery, conn))
                        {
                            cmdMusteri.Parameters.AddWithValue("@KisiId", kisiId);
                            cmdMusteri.Parameters.AddWithValue("@MusteriTuru", kisituru); // Replace with actual value
                            cmdMusteri.Transaction = tran;
                            cmdMusteri.ExecuteNonQuery();
                        }

                        tran.Commit();

                        MessageBox.Show("Kayıt başarıyla eklendi.");
                    }
                    catch (Npgsql.PostgresException ex)
                    {
                        tran.Rollback();

                        if (ex.SqlState == "23505") // SQLSTATE code 23505 means unique key violation
                        {
                            MessageBox.Show("Bu özelliklerde bir kişi zaten bulunmakta!");
                        }
                        else
                        {
                            MessageBox.Show("Bir hata oluştu: " + ex.Message);
                        }
                    }
                }
            }
        

            private void airButton3_Click(object sender, EventArgs e)
            {
                try {
                    // DataGridView'dan seçili satırı kontrol et
                    if (dataGridView2.SelectedRows.Count > 0)
                    {
                        // Seçili satırın indeksini al
                        int rowIndex = dataGridView2.SelectedRows[0].Index;

                        // Seçili satırın ilgili hücrelerinden değerleri al

                        string ad = dataGridView2.Rows[rowIndex].Cells["ad"].Value.ToString();
                        string soyad = dataGridView2.Rows[rowIndex].Cells["soyad"].Value.ToString();
                        string tckn = dataGridView2.Rows[rowIndex].Cells["tckn"].Value.ToString();
                        string cinsiyet = dataGridView2.Rows[rowIndex].Cells["cinsiyet"].Value.ToString();
                        string telefon = dataGridView2.Rows[rowIndex].Cells["telefon"].Value.ToString();
                        string sifre = dataGridView2.Rows[rowIndex].Cells["sifre"].Value.ToString();
                        string kisituru = dataGridView2.Rows[rowIndex].Cells["kisituru"].Value.ToString();



                        // Yeni değerleri almak için kullanılacak kontrolleri ekleyin
                        string Yeniad = (dreamTextBox4.Text); // TextBox'tan alınacak
                        string Ysoyad = (dreamTextBox10.Text); // TextBox'tan alınacak
                        string Ytckn = (dreamTextBox13.Text); // TextBox'tan alınacak
                        string Ycinsiyet = (dreamTextBox11.Text);
                        string Ytelefon = (dreamTextBox3.Text);
                        string Ysifre = (dreamTextBox12.Text);
                        string Ykisituru = (dreamTextBox2.Text);
                        // Yeni değerleri DataGridView'daki ilgili hücrelere ata
                        dataGridView2.Rows[rowIndex].Cells["ad"].Value = Yeniad;
                        dataGridView2.Rows[rowIndex].Cells["soyad"].Value = Ysoyad;
                        dataGridView2.Rows[rowIndex].Cells["tckn"].Value = Ytckn;
                        dataGridView2.Rows[rowIndex].Cells["dogumtarihi"].Value = dateTimePicker1.Value.Date;
                        dataGridView2.Rows[rowIndex].Cells["cinsiyet"].Value = Convert.ToChar(Ycinsiyet);
                        dataGridView2.Rows[rowIndex].Cells["telefon"].Value = Ytelefon;
                        dataGridView2.Rows[rowIndex].Cells["sifre"].Value = Ysifre;
                        dataGridView2.Rows[rowIndex].Cells["kisituru"].Value = Ykisituru;

                        // Veritabanında güncelleme işlemini gerçekleştir
                        using (NpgsqlConnection conn = new NpgsqlConnection(baglanti))
                        {
                            try
                            {
                                conn.Open();

                                string query = "UPDATE public.\"kisi\" SET \"ad\" = @ad, \"soyad\" = @soyad, \"tckn\" = @tckn \"dogumtarihi\"=@dogumtarihi \"cinsiyet\"=@cinsiyet \"telefon\"=@telefon \"sifre\"=@sifre \"kisituru\"=@kisituru WHERE \"TCKN\" = @TCKN";
                                NpgsqlCommand npgsqlCommand = new NpgsqlCommand(query, conn);

                                // Parametreleri ekleyin
                                npgsqlCommand.Parameters.AddWithValue("@ad", Yeniad);
                                npgsqlCommand.Parameters.AddWithValue("@soyad", Ysoyad);
                                npgsqlCommand.Parameters.AddWithValue("@tckn", Ytckn);
                                npgsqlCommand.Parameters.AddWithValue("@dogumtarihi", dateTimePicker1.Value.Date);
                                npgsqlCommand.Parameters.AddWithValue("@cinsiyet", Ycinsiyet);
                                npgsqlCommand.Parameters.AddWithValue("@telefon", Ytelefon);
                                npgsqlCommand.Parameters.AddWithValue("@sifre", Ysifre);
                                npgsqlCommand.Parameters.AddWithValue("@kisituru", Ykisituru);

                                // Komutu çalıştırın
                                int affectedRows = npgsqlCommand.ExecuteNonQuery();

                                if (affectedRows > 0)
                                {
                                    MessageBox.Show("Veri başarıyla güncellendi.");
                                }
                                else
                                {
                                    MessageBox.Show("Veri güncellenirken bir hata oluştu.");
                                }
                            }
                            catch (Npgsql.PostgresException ex)
                            {
                                MessageBox.Show("Bir hata oluştu: " + ex.Message);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Lütfen güncellenecek bir satır seçin.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                }
            }

            private void airButton4_Click(object sender, EventArgs e)
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(baglanti))
                {
                    try
                    {
                        conn.Open();

                        // DELETE sorgusunu oluşturun (örneğin, "doviz" tablosundan silme)
                        string query = "DELETE FROM public.\"kisi\" WHERE \"tckn\" = @tckn";
                        NpgsqlCommand npgsqlCommand = new NpgsqlCommand(query, conn);

                        // Parametre ekleyin
                        npgsqlCommand.Parameters.AddWithValue("@tckn", aloneTextBox2.Text);

                        // Komutu çalıştırın
                        int affectedRows = npgsqlCommand.ExecuteNonQuery();

                        if (affectedRows > 0)
                        {
                            MessageBox.Show("Kayıt başarıyla silindi.");
                        }
                        else
                        {
                            MessageBox.Show("Belirtilen TCKNO'ya sahip kayıt bulunamadı veya silinirken bir hata oluştu.");
                        }
                    }
                    catch (Npgsql.PostgresException ex)
                    {
                        MessageBox.Show("Bir hata oluştu: " + ex.Message);
                    }
                }
            }

            private void tabPage1_Click(object sender, EventArgs e)
            {

            }

            private void SearchBt_Click(object sender, EventArgs e)
            {
                string Doviz = aloneTextBox1.Text; // TextBox'tan Doviz bilgisini alın, txtDoviz TextBox'ınızın adı olabilir

                if (string.IsNullOrWhiteSpace(Doviz))
                {
                    MessageBox.Show("Doviz Kutusu boş olamaz.");
                    return;
                }
                try {
                    using (NpgsqlConnection conn = new NpgsqlConnection(baglanti))
                    {
                        conn.Open();

                        // Kullanıcı adı ve şifreyi kontrol etmek için sorgu
                        string query = "SELECT * FROM public.\"doviz\" WHERE \"dovizadi\" = @Doviz";
                        NpgsqlCommand npgsqlCommand = new NpgsqlCommand(query, conn);
                        npgsqlCommand.Parameters.AddWithValue("@Doviz", Doviz);

                        NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(npgsqlCommand);
                        DataSet ds = new DataSet();
                        dataAdapter.Fill(ds);

                        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        {
                            dataGridView1.DataSource = ds.Tables[0];
                            MessageBox.Show("Doviz bulundu.");
                        }
                        else
                        {
                            MessageBox.Show("Doviz bulunamadı.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                }
            }


            private void GetDoviz(string connString, DataGridView dgv)
            {
                try {
                    NpgsqlConnection conn = new NpgsqlConnection(connString);
                    conn.Open();

                    string query = "SELECT * FROM public.\"doviz\"";

                    NpgsqlCommand npgsqlCommand = new NpgsqlCommand(query, conn);
                    NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(npgsqlCommand);

                    DataSet ds = new DataSet();
                    dataAdapter.Fill(ds);

                    dgv.DataSource = ds.Tables[0];
                    conn.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                }
            }
            private void dataGridView1_SelectionChanged(object sender, EventArgs e)
            {
                try {
                    if (dataGridView1.SelectedRows.Count > 0)
                    {
                        DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

                        // Seçilen satırın verilerine erişim örnek:
                        dreamTextBox8.Text = selectedRow.Cells["aliskuru"].Value.ToString();
                        dreamTextBox6.Text = selectedRow.Cells["satiskuru"].Value.ToString();
                        dreamTextBox9.Text = selectedRow.Cells["rezerv"].Value.ToString();

                        // Burada seçilen satırın verileri ile istediğiniz işlemleri gerçekleştirebilirsiniz.
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                }
            }


            private void airButton6_Click(object sender, EventArgs e)
            {
                GetDoviz(baglanti, dataGridView1);
            }

            private void Ekle_Click(object sender, EventArgs e)
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(baglanti))
                {
                    try
                    {
                        conn.Open();

                        // INSERT sorgusunu oluşturun (örneğin, "doviz" tablosuna ekleme)
                        string query = "INSERT INTO public.\"doviz\" (\"dovizadi\", \"aliskuru\", \"satiskuru\", \"rezerv\") VALUES (@dovizadi, 0, 0, 1000000)";
                        NpgsqlCommand npgsqlCommand = new NpgsqlCommand(query, conn);

                        // Parametreleri ekleyin
                        npgsqlCommand.Parameters.AddWithValue("@dovizadi", aloneTextBox1.Text); // aloneTextBox1 TextBox'ınızın adı olabilir
                        npgsqlCommand.Parameters.AddWithValue("@aliskuru", 0);
                        npgsqlCommand.Parameters.AddWithValue("@satiskuru", 0);
                        npgsqlCommand.Parameters.AddWithValue("@rezerv", 1000000);

                        // Komutu çalıştırın
                        int affectedRows = npgsqlCommand.ExecuteNonQuery();

                        if (affectedRows > 0)
                        {
                            MessageBox.Show("Kayıt başarıyla eklendi.");
                        }
                        else
                        {
                            MessageBox.Show("Kayıt eklenirken bir hata oluştu.");
                        }
                    }
                    catch (Npgsql.PostgresException ex)
                    {
                        if (ex.SqlState == "23505") // SQLSTATE kodu 23505, unique key ihlali anlamına gelir
                        {
                            MessageBox.Show("Bu dovizadi zaten kullanılıyor. Lütfen başka bir dovizadi seçin.");
                        }
                        else
                        {
                            MessageBox.Show("Bir hata oluştu: " + ex.Message);
                        }
                    }
                }
            }

            private void Sil_Click(object sender, EventArgs e)
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(baglanti))
                {
                    try
                    {
                        conn.Open();

                        // DELETE sorgusunu oluşturun (örneğin, "doviz" tablosundan silme)
                        string query = "DELETE FROM public.\"doviz\" WHERE \"dovizadi\" = @DovizAdi";
                        NpgsqlCommand npgsqlCommand = new NpgsqlCommand(query, conn);

                        // Parametre ekleyin
                        npgsqlCommand.Parameters.AddWithValue("@DovizAdi", aloneTextBox1.Text);

                        // Komutu çalıştırın
                        int affectedRows = npgsqlCommand.ExecuteNonQuery();

                        if (affectedRows > 0)
                        {
                            MessageBox.Show("Kayıt başarıyla silindi.");
                        }
                        else
                        {
                            MessageBox.Show("Belirtilen dovizadi'ye sahip kayıt bulunamadı veya silinirken bir hata oluştu.");
                        }
                    }
                    catch (Npgsql.PostgresException ex)
                    {
                        MessageBox.Show("Bir hata oluştu: " + ex.Message);
                    }
                }
            }

            private void Update_Click(object sender, EventArgs e)
            {
                // DataGridView'dan seçili satırı kontrol et
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    // Seçili satırın indeksini al
                    int rowIndex = dataGridView1.SelectedRows[0].Index;

                    // Seçili satırın ilgili hücrelerinden değerleri al
                    string dovizAdi = dataGridView1.Rows[rowIndex].Cells["dovizadi"].Value.ToString();
                    decimal alisKuru = Convert.ToDecimal(dataGridView1.Rows[rowIndex].Cells["aliskuru"].Value);
                    decimal satisKuru = Convert.ToDecimal(dataGridView1.Rows[rowIndex].Cells["satiskuru"].Value);
                    int rezerv = Convert.ToInt32(dataGridView1.Rows[rowIndex].Cells["rezerv"].Value);

                    // Güncelleme yapmak için TextBox'ları ve diğer kontrolleri kullanabilirsiniz
                    // Örneğin, TextBox'lar üzerinden kullanıcının girdiği yeni değerleri alabilirsiniz

                    // Örneğin, güncelleme için yeni değerleri almak üzere MessageBox kullanabilirsiniz
                    // MessageBox.Show("Lütfen güncellenmiş değerleri girin.");

                    // Yeni değerleri almak için kullanılacak kontrolleri ekleyin
                    decimal yeniAlisKuru = Convert.ToDecimal(dreamTextBox7.Text); // TextBox'tan alınacak
                    decimal yeniSatisKuru = Convert.ToDecimal(dreamTextBox5.Text); // TextBox'tan alınacak
                    int yeniRezerv = Convert.ToInt32(dreamTextBox1.Text); // TextBox'tan alınacak

                    // Yeni değerleri DataGridView'daki ilgili hücrelere ata
                    dataGridView1.Rows[rowIndex].Cells["aliskuru"].Value = yeniAlisKuru;
                    dataGridView1.Rows[rowIndex].Cells["satiskuru"].Value = yeniSatisKuru;
                    dataGridView1.Rows[rowIndex].Cells["rezerv"].Value = yeniRezerv;

                    // Veritabanında güncelleme işlemini gerçekleştir
                    using (NpgsqlConnection conn = new NpgsqlConnection(baglanti))
                    {
                        try
                        {
                            conn.Open();

                            // UPDATE sorgusunu oluşturun
                            string query = "UPDATE public.\"doviz\" SET \"aliskuru\" = @AlisKuru, \"satiskuru\" = @SatisKuru, \"rezerv\" = @Rezerv WHERE \"dovizadi\" = @DovizAdi";
                            NpgsqlCommand npgsqlCommand = new NpgsqlCommand(query, conn);

                            // Parametreleri ekleyin
                            npgsqlCommand.Parameters.AddWithValue("@AlisKuru", yeniAlisKuru);
                            npgsqlCommand.Parameters.AddWithValue("@SatisKuru", yeniSatisKuru);
                            npgsqlCommand.Parameters.AddWithValue("@Rezerv", yeniRezerv);
                            npgsqlCommand.Parameters.AddWithValue("@DovizAdi", dovizAdi);

                            // Komutu çalıştırın
                            int affectedRows = npgsqlCommand.ExecuteNonQuery();

                            if (affectedRows > 0)
                            {
                                MessageBox.Show("Veri başarıyla güncellendi.");
                            }
                            else
                            {
                                MessageBox.Show("Veri güncellenirken bir hata oluştu.");
                            }
                        }
                        catch (Npgsql.PostgresException ex)
                        {
                            MessageBox.Show("Bir hata oluştu: " + ex.Message);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Lütfen güncellenecek bir satır seçin.");
                }
            }

            private void dreamTextBox10_TextChanged(object sender, EventArgs e)
            {

            }

            private void airButton5_Click(object sender, EventArgs e)
            {
                try {
                    NpgsqlConnection conn = new NpgsqlConnection(baglanti);
                    conn.Open();

                    string query = "SELECT * FROM public.\"kisi\"";

                    NpgsqlCommand npgsqlCommand = new NpgsqlCommand(query, conn);
                    NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(npgsqlCommand);

                    DataSet ds = new DataSet();
                    dataAdapter.Fill(ds);

                    dataGridView2.DataSource = ds.Tables[0];
                    conn.Close();
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

                    // Seçilen satırın verilerine erişim örnek:
                    dreamTextBox4.Text = selectedRow.Cells["ad"].Value.ToString();
                    dreamTextBox10.Text = selectedRow.Cells["soyad"].Value.ToString();
                    dreamTextBox13.Text = selectedRow.Cells["tckn"].Value.ToString();
                    dreamTextBox11.Text = selectedRow.Cells["cinsiyet"].Value.ToString();
                    dreamTextBox3.Text = selectedRow.Cells["telefon"].Value.ToString();
                    dreamTextBox12.Text = selectedRow.Cells["sifre"].Value.ToString();
                    dreamTextBox2.Text = selectedRow.Cells["kisituru"].Value.ToString();

                    // UPDATE sorgusunu oluşturunpublic.\"kisi\" (\"ad\", \"soyad\", \"tckn\", \"dogumtarihi\",\"cinsiyet\",\"telefon\",\"sifre\",\"kisituru\")

                    // Burada seçilen satırın verileri ile istediğiniz işlemleri gerçekleştirebilirsiniz.
                }
            }
            string hesapTuru = "";
            private void airButton1_Click_1(object sender, EventArgs e)
            {
                HesapEkle(hesapTuru, HesapText.Text, Convert.ToDecimal(BakiyeText.Text), FaizTD, Convert.ToDecimal(FaizText.Text), Convert.ToInt32(aloneTextBox3.Text));
            }
            private void HesapEkle(string hesapTuru, string hesapNumarasi, decimal bakiye, DateTimePicker vadeTarihi, decimal faizOrani, int musteriId)
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(baglanti))
                {
                    try
                    {
                        conn.Open();

                        if (hesapTuru == "Doviz")
                        {
                            using (NpgsqlTransaction transaction = conn.BeginTransaction())
                            {
                                try
                                {
                                    // Step 1: Insert into Hesap table
                                    string insertHesapQuery = "INSERT INTO Hesap (MusteriId, HesapNumarasi, OlusturulmaTarihi, HesapTuru) VALUES (@p_musteriid, @p_hesapnumarasi, @p_olusturulmatarihi, @p_hesapturu) RETURNING HesapId";
                                    using (NpgsqlCommand cmdHesap = new NpgsqlCommand(insertHesapQuery, conn, transaction))
                                    {
                                        cmdHesap.Parameters.AddWithValue("@p_musteriid", musteriId);
                                        cmdHesap.Parameters.AddWithValue("@p_hesapnumarasi", hesapNumarasi);
                                        cmdHesap.Parameters.AddWithValue("@p_olusturulmatarihi", DateTime.Now);
                                        cmdHesap.Parameters.AddWithValue("@p_hesapturu", hesapTuru);

                                        // Execute the command and get the generated HesapId
                                        int hesapId = (int)cmdHesap.ExecuteScalar();

                                        // Step 2: Insert into DovizHesap table
                                        string insertDovizHesapQuery = "INSERT INTO DovizHesap (DovizHesapID, Miktar, DovizID) VALUES (@p_dovizhesapid, @p_miktar, @p_dovizid)";
                                        using (NpgsqlCommand cmdDovizHesap = new NpgsqlCommand(insertDovizHesapQuery, conn, transaction))
                                        {
                                            cmdDovizHesap.Parameters.AddWithValue("@p_dovizhesapid", hesapId); // Use the generated HesapId
                                            cmdDovizHesap.Parameters.AddWithValue("@p_miktar", bakiye);
                                            cmdDovizHesap.Parameters.AddWithValue("@p_dovizid", int.Parse(DovizText.Text));

                                            cmdDovizHesap.ExecuteNonQuery();
                                        }

                                        // Commit the transaction
                                        transaction.Commit();

                                        MessageBox.Show("İşlem başarıyla tamamlandı.", "Başarı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    // Rollback the transaction in case of an exception
                                    transaction.Rollback();
                                    MessageBox.Show("Hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }

                        else
                        {
                            {
                                if (faizOrani == 0)
                                    faizOrani = 1;
                                // Hesap tablosuna ekleme yap
                                string hesapEkleQuery = "INSERT INTO public.\"hesap\" (\"musteriid\",\"hesapnumarasi\", \"olusturulmatarihi\", \"hesapturu\") VALUES (@musteriid, @hesapnumarasi, @olusturulmatarihi, @hesapturu) RETURNING hesapid";
                                using (NpgsqlCommand hesapEkleCommand = new NpgsqlCommand(hesapEkleQuery, conn))
                                {
                                    hesapEkleCommand.Parameters.AddWithValue("@musteriId", musteriId);
                                    hesapEkleCommand.Parameters.AddWithValue("@hesapnumarasi", hesapNumarasi);
                                    hesapEkleCommand.Parameters.AddWithValue("@olusturulmatarihi", DateTime.Now);
                                    hesapEkleCommand.Parameters.AddWithValue("@hesapturu", hesapTuru);

                                    int hesapId = (int)hesapEkleCommand.ExecuteScalar();

                                    // Hesap türüne göre ilgili tabloya ekleme yap
                                    if (hesapTuru == "Vadesiz")
                                    {
                                        string vadesizEkleQuery = "INSERT INTO public.\"vadesizhesap\" (\"hesapid\", \"bakiye\") VALUES (@hesapid, @bakiye)";
                                        using (NpgsqlCommand vadesizEkleCommand = new NpgsqlCommand(vadesizEkleQuery, conn))
                                        {
                                            vadesizEkleCommand.Parameters.AddWithValue("@hesapid", hesapId);
                                            vadesizEkleCommand.Parameters.AddWithValue("@bakiye", bakiye);
                                            vadesizEkleCommand.ExecuteNonQuery();
                                        }
                                    }
                                    else if (hesapTuru == "Vadeli")
                                    {
                                        string vadeliEkleQuery = "INSERT INTO public.\"vadelihesap\" (\"hesapid\", \"bakiye\", \"vadetarihi\", \"faizorani\") VALUES (@hesapid, @bakiye, @vadetarihi, @faizorani)";
                                        using (NpgsqlCommand vadeliEkleCommand = new NpgsqlCommand(vadeliEkleQuery, conn))
                                        {
                                            vadeliEkleCommand.Parameters.AddWithValue("@hesapid", hesapId);
                                            vadeliEkleCommand.Parameters.AddWithValue("@bakiye", bakiye);
                                            vadeliEkleCommand.Parameters.AddWithValue("@vadeTarihi", vadeTarihi.Value.Date);
                                            vadeliEkleCommand.Parameters.AddWithValue("@faizOrani", faizOrani);
                                            vadeliEkleCommand.ExecuteNonQuery();
                                        }
                                    }
                                    /* if (hesapTuru == "Doviz")
                                     {
                                         try
                                         {
                                             // Kullanıcının girdiği değerleri al
                                             decimal miktar = decimal.Parse(Bakiye.Text);
                                             int dovizID = int.Parse(DovizText.Text);

                                             string insertQuery = "call ekle_doviz_hesap(@musteriid,@hesapno,@olusturulmatarihi,)";

                                             using (NpgsqlCommand cmd = new NpgsqlCommand(insertQuery, conn))
                                             {
                                                 cmd.Parameters.AddWithValue("@miktar", miktar);
                                                 cmd.Parameters.AddWithValue("@dovizid", dovizID);
                                                 cmd.Parameters.AddWithValue("@hesapId", hesapId);

                                                 cmd.ExecuteNonQuery();

                                                 MessageBox.Show("Döviz Hesabı başarıyla eklendi.", "Başarı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                             }
                                         }
                                         catch (Exception ex)
                                         {
                                             MessageBox.Show("Hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                         }*/
                                }
                            }
                        }




                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }
            }
            private void VadeliHesap_MouseClick(object sender, MouseEventArgs e)
            {
                if (VadesizHesap.Enabled)
                {
                    Bakiye.Visible = true;
                    BakiyeText.Visible = true;
                    DovizText.Visible = false;
                    DovizID.Visible = false;
                    FaizOrani.Visible = true;
                    FaizText.Visible = true;
                    hesapTuru = "Vadeli";
                }
            }

            private void DovizHesap_MouseClick(object sender, MouseEventArgs e)
            {
                if (DovizHesap.Enabled)
                {
                    Bakiye.Visible = false;
                    BakiyeText.Visible = false;
                    DovizText.Visible = true;
                    DovizID.Visible = true;
                    FaizOrani.Visible = false;
                    FaizText.Visible = false;
                    hesapTuru = "Doviz";
                }
            }

            private void VadesizHesap_MouseClick(object sender, MouseEventArgs e)
            {
                if (VadeliHesap.Enabled)
                {
                    FaizText.Visible = false;
                    FaizOrani.Visible = false;
                    DovizText.Visible = false;
                    DovizID.Visible = false;
                    Bakiye.Visible = true;
                    BakiyeText.Visible = true;
                    FaizTD.Visible = false;
                    dungeonHeaderLabel17.Visible = false;
                    hesapTuru = "Vadesiz";
                }
            }

            private void YDovizGetir_Click(object sender, EventArgs e)
            {
                GetDoviz(baglanti, dataGridView3);
            }
        string kisituru = "";
        private void comboBoxEdit1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxEdit1.SelectedItem.ToString() == "Bireysel")
            {
                kisituru = "Bireysel";
            }
            if (comboBoxEdit1.SelectedItem.ToString() == "Kurumsal")
            {
                kisituru = "Kurumsal";
            }
        }
    }
    } 
