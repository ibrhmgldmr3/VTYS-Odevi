using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VTYSODEV2
{
    public partial class HesapIslemleri : Form
    {
        string baglanti = "Server=localhost;Port=5432;Database=Banka2;User Id=postgres;Password=0326;";
        HesapIslemleri()
        {
            
        }

        public HesapIslemleri(int hesapid)
        {
            InitializeComponent();
            ListIslemler(hesapid);
        }

        private void HesapIslemleri_Load(object sender, EventArgs e)
        {

        }
        private void ListIslemler(int hesapId)
        {
            try
            {
                using (var conn = new NpgsqlConnection(baglanti))
                {
                    conn.Open();

                    string query = "SELECT Islem.IslemID,Hesap.HesapNumarasi,IslemTuru.TurAdi AS IslemTuru,Islem.Miktar,Islem.Tarih FROM Islem JOIN Hesap ON Islem.HesapID = Hesap.HesapID JOIN IslemTuru ON Islem.IslemTuruID = IslemTuru.IslemTuruID WHERE Islem.HesapID = @hesapid"
                        +" order by Islem.Tarih desc";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@hesapId", hesapId);

                        using (var dataAdapter = new NpgsqlDataAdapter(cmd))
                        {
                            var ds = new DataSet();
                            dataAdapter.Fill(ds);

                            dataGridView1.DataSource = ds.Tables[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }
    }
}
