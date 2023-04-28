using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Timers;
using System.IO;

namespace gunlukrapor
{
    public partial class EnverServis : ServiceBase
    {

        static string yil = "";
        SqlConnection baglanti = new SqlConnection("Data Source=HPSERVER\\ETA;Initial Catalog=ETA_MARKET_" + yil.ToString() +";user id=sa;password=eta_123456;");
        SqlConnection baglanti2 = new SqlConnection("Data Source=HPSERVER\\ETA;Initial Catalog=ETA_RAPORLAR_"+ yil.ToString() + ";user id=sa;password=eta_123456;");
        Timer timer = new Timer();
        public EnverServis()
        {
            InitializeComponent();
        }
        protected override void OnStart(string[] args)
        {
            yil=DateTime.Now.Year.ToString();
            string saat = DateTime.Now.Hour.ToString();
            if (saat.Equals("20"))
            {
                RaporAl();
                timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
                timer.Interval = 3600000;
                LogTut("\nBAŞLADI  |  Günlük Rapor Alma Başarılı...", DateTime.Now.ToString());
            }
            else
            {
                timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
                timer.Interval = 3600000;
                LogTut("\nBAŞLADI  |  Rapor Alınamadı Saat 20 değil...'" + saat + "'", DateTime.Now.ToString());
            } 
            timer.Enabled = true;
        }

        protected override void OnStop()
        {
            LogTut("\nHizmet Durduruldu.", DateTime.Now.ToString());
        }
        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            yil = DateTime.Now.Year.ToString();
            string saat = DateTime.Now.Hour.ToString();
            if (saat.Equals("10"))
            {
                RaporAl();
                timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
                timer.Interval = 3600000;
                LogTut("DEVAM EDİYOR  |  Günlük Rapor Alma Başarılı...", DateTime.Now.ToString());
            }
            else
            {
                timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
                timer.Interval = 3600000;
                LogTut("DEVAM EDİYOR  |  Rapor Alınamadı Saat 20 değil...'" + saat + "'", DateTime.Now.ToString());
            }
            timer.Enabled = true;
        }

        public void LogTut(string mesaj, string zaman)
        {
            string dosyayolu = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            string textyolu = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\" + DateTime.Now.Date.ToString().Substring(0, 10) + ".txt";
            if (!Directory.Exists(dosyayolu))
            {
                Directory.CreateDirectory(dosyayolu);
            }
            if (!File.Exists(textyolu))
            {
                using (StreamWriter sw = File.CreateText(textyolu))
                {
                    sw.WriteLine(zaman + "  |  " + mesaj);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(textyolu))
                {
                    sw.WriteLine(zaman + "  |  " + mesaj);
                }
            }
        }

        public void RaporAl()

        {
            baglanti.Close();
            baglanti.Open();
            string tarih = "0",islemtarih=DateTime.Now.ToString();
            decimal alistop = 0, satistop = 0, topkar = 0, karyuzde = 0;
            SqlCommand raporal = new SqlCommand("SELECT *FROM SAIT_GUNLUKRAPOR", baglanti);
            SqlDataReader dr2 = raporal.ExecuteReader();
            if (dr2.Read())
            {
                tarih = dr2["TARIH"].ToString().Substring(0, 10);
                alistop = Convert.ToDecimal(dr2["ALISTOPLAM"]);
                satistop = Convert.ToDecimal(dr2["SATISTOPLAM"]);
                topkar = Convert.ToDecimal(dr2["TOPLAMKAR"]);
                karyuzde = Convert.ToDecimal(dr2["KARYUZDE"]);
            }
            baglanti.Close();
            baglanti2.Close();
            baglanti2.Open();
            tarih = DateTime.Now.Date.ToString("dd.MM.yyyy");
            SqlCommand gunlukraporkayit = new SqlCommand("INSERT INTO GUNLUKRAPOR(gun_tarih,gun_kartutar,gun_cirotutar,gun_karyuzde,gun_islemtarih) " +
                "values(@tarih,@topkar,@satistop,@karyuzde,@islemtarih)", baglanti2);
            gunlukraporkayit.Parameters.AddWithValue("@tarih", tarih);
            gunlukraporkayit.Parameters.AddWithValue("@topkar", topkar);
            gunlukraporkayit.Parameters.AddWithValue("@satistop", satistop);
            gunlukraporkayit.Parameters.AddWithValue("@karyuzde", karyuzde);
            gunlukraporkayit.Parameters.AddWithValue("@islemtarih", islemtarih);
            gunlukraporkayit.ExecuteNonQuery();
            baglanti2.Close();
            
        }
    }
}
