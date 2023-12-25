using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace VTYSODEV2
{
    public partial class Login : Form
    {
        string baglanti = "Server=localhost;Port=5432;Database=Banka2;User Id=postgres;Password=0326;";
        public Login()
        {
            InitializeComponent();
            dreamTextBox1.PasswordChar = '*';
            dreamTextBox3.PasswordChar = '*';
        }
        string _kisituru = "M";
        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (CheckLogin(baglanti, dreamTextBox4.Text, dreamTextBox3.Text))
            {
                YoneticiPanel YoneticiPanel = new YoneticiPanel();
                YoneticiPanel.Show();
            }
            else
            {
                MessageBox.Show("Giriş başarısız.");
            }
        }
        private bool CheckLogin(string connString, string kisiid, string sifre)
        {
            if (string.IsNullOrWhiteSpace(kisiid) || string.IsNullOrWhiteSpace(sifre))
            {
                MessageBox.Show("Kullanıcı adı ve şifre boş olamaz.");
                return false;
            }

            NpgsqlConnection conn = new NpgsqlConnection(connString);
            
            conn.Open();
                // Kullanıcı adı ve şifreyi kontrol etmek için sorgu
            string query = "SELECT COUNT(*) FROM public.\"kisi\" where \"kisiid\"=@kisiid and \"sifre\"=@Sifre and \"kisituru\"=@kisituru";
            NpgsqlCommand npgsqlCommand = new NpgsqlCommand(query, conn);
            npgsqlCommand.Parameters.AddWithValue("kisiid", Convert.ToInt64(kisiid));
            npgsqlCommand.Parameters.AddWithValue("sifre", sifre);
            npgsqlCommand.Parameters.AddWithValue("kisituru", _kisituru);
            object kisituru = npgsqlCommand.ExecuteScalar();
                int sayi =Convert.ToInt32(npgsqlCommand.ExecuteScalar());
            if (sayi > 0)
            {
                return true;
            }
            return false;
        }
        

        private void button1_Click(object sender, EventArgs e)
        {
            if (CheckLogin(baglanti, dreamTextBox2.Text, dreamTextBox1.Text))
            {
                MusteriPanel MusteriPanel = new MusteriPanel(dreamTextBox2.Text);
                MusteriPanel.Show();
            }
            else
            {
                MessageBox.Show("Giriş başarısız.");
            }
        }

        private void hopeTabPage1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            if (hopeTabPage1.SelectedIndex == 0)
            {
                _kisituru = "M";
            }
            else
                _kisituru = "C";
        }
    }
}
