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

namespace gunlukrapor
{
    public partial class EnverServis : ServiceBase
    {

        static string yil = "";
        SqlConnection baglanti = new SqlConnection("Data Source=HPSERVER\\ETA;Initial Catalog=ETA_MARKET_" + yil.ToString() +";user id=sa;password=eta_123456;");
        SqlConnection baglanti2 = new SqlConnection("Data Source=HPSERVER\\ETA;Initial Catalog=ETA_RAPORLAR_"+ yil.ToString() + ";user id=sa;password=eta_123456;");
        Timer timer = new Timer();
        Timer timer2 = new Timer();
        public EnverServis()
        {
            InitializeComponent();
        }
        protected override void OnStart(string[] args)
        {
            yil=DateTime.Now.Year.ToString();
            string saat = DateTime.Now.Hour.ToString();
            if (saat == "20")
            {
                timer2.Enabled = false;
                RaporAl();
                timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
                timer.Interval = 3600000;
                timer.Enabled = true;
            }
            else
            {
                timer.Enabled=false;
                timer2.Elapsed += new ElapsedEventHandler(OnElapsedTime2);
                timer2.Interval = 3600000;
                timer2.Enabled = true;
            }
        }

        protected override void OnStop()
        {
        }
        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            yil = DateTime.Now.Year.ToString();
            RaporAl();
        }
        private void OnElapsedTime2(object source, ElapsedEventArgs e)
        {
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
