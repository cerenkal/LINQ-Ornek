using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LINQ_Ornek
{
    public partial class Form1 : Form
    {
        NorthwindEntities db;
        public Form1()
        {
            InitializeComponent();
           db = new NorthwindEntities();
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            //Toplam Kazanç bilgisini hesaplayıp kullanıcıya MessageBox ta gösteriniz
            var quantityList = db.Order_Details.Select(x => x.Quantity).ToList(); //adet bütün sipaişler için
            var unitPriceList = db.Order_Details.Select(x => x.UnitPrice).ToList(); // birim fiyat bütün siparişler için 
            var discountList = db.Order_Details.Select(x => x.Discount).ToList(); //indirim bütün siparişler için

            decimal toplamFiyat = 0;
            for (int i = 0; i < quantityList.Count()-1; i++)
            {
                toplamFiyat += quantityList[i] * unitPriceList[i] * (decimal)(1 - discountList[i]);
            }
            MessageBox.Show(toplamFiyat.ToString());  
                
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //hangi çalışanım hangi çalışanıma bağlı datagridview de gösteriniz
            dataGridView1.DataSource = db.Employees
                .Select(x => new
                {
                    Calisan_adi=x.FirstName,
                    Bagli_Oldugu_Calisan=x.Employee1.FirstName
                }).ToList();
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //Nancy çalışanım ne kadarlık satış yaptığını bulan kodu yazınız ve messageBox ta gösteriniz
            var arananPersonelID = db.Employees.Where(x => x.FirstName == "Nancy").Select(x => x.EmployeeID).FirstOrDefault();

            var nancySatişlari = db.Order_Details.Where(x => x.Order.Employee.EmployeeID == arananPersonelID).ToList();

            decimal toplamSatisKari = 0;
            foreach (var item in nancySatişlari)
            {
                toplamSatisKari += (item.UnitPrice*item.Quantity);
            }
            MessageBox.Show(toplamSatisKari.ToString());
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //her bir kategori stoktaki ürün miktarını listeleyiniz. datagridview de gösteriniz
            dataGridView1.DataSource = db.Products.GroupBy(x => x.Category.CategoryName).Select(x => new
            {
                Kategori_Adi = x.Key,
                Toplam_Urun_Miktari = x.Sum(y => y.UnitsInStock)
            }).ToList();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //250 den fazla sipariş taşımış olan kargo firmalarını listeleyiniz(telefon no ve company bilgisine göre group by yapınız)
            dataGridView1.DataSource = db.Orders.GroupBy(x => new { x.Shipper.CompanyName, x.Shipper.Phone }).Select(y => new
            {
                Firma_Adi = y.Key.CompanyName,
                Firma_Telefon = y.Key.Phone,
                Toplam_Siparis_Sayisi = y.Count()
            }).Where(z => z.Toplam_Siparis_Sayisi > 250).ToList();

            
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //faks numarası null olan müşterilerimi listele
            dataGridView1.DataSource = db.Customers
                .Where(x => x.Fax == null).ToList();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            // en pahalı 5 ürünün getrilmesini sağlayınız burada if ve break kullanın
            var urunlerList = db.Products.OrderByDescending(x => x.UnitPrice).ToList();

            int sayac = 0;
            foreach (var item in urunlerList)
            {
                MessageBox.Show(item.ProductName + " " + item.UnitPrice.ToString());
                sayac++;
                if (sayac == 5)
                    break; //şart sağlanırsa döngünün sonlandırılmasını sağlıyoruz.
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            //en az birim fiyata sahip olan ürün adını messagebox'ta gösteriniz
            var urunlerListedir = db.Products.OrderByDescending(x => x.UnitPrice).ToList();
            MessageBox.Show(urunlerListedir[urunlerListedir.Count() - 1].ProductName.ToString() + " " + urunlerListedir[urunlerListedir.Count() - 1].UnitPrice.ToString());

            //VEYA//
            var urunlerListesi = db.Products.OrderByDescending(x => x.UnitPrice).ToList();
            var enUcuzUrun = urunlerListedir[urunlerListedir.Count() - 1];
            Product enUcuzUrun2 = urunlerListedir[urunlerListedir.Count() - 1];

            MessageBox.Show(enUcuzUrun.ProductName + " " + enUcuzUrun2.UnitPrice.ToString());

            //VEYA//
            var urunlerListesi2 = db.Products.OrderBy(x => x.UnitPrice).ToList();
            for (int i = urunlerListesi2.Count()-1; i < urunlerListesi2.Count(); i++)
            {
                MessageBox.Show(urunlerListesi2[urunlerListesi2.Count() - 1].ProductName.ToString() + " " + urunlerListesi2[urunlerListesi2.Count() - 1].UnitPrice.ToString());
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            //region bilgisi western olan konumları getiriniz Chicago= western (Region ile Territories)

            var regionID = db.Regions.Where(x => x.RegionDescription == "Western").Select(x => x.RegionID).FirstOrDefault();
            dataGridView1.DataSource = db.Territories.Where(x => x.RegionID == regionID).Select(x => new
            {
                BolgeID = x.RegionID.ToString(),
                Bolge_Adi=x.Region.RegionDescription,
                Sehir=x.TerritoryDescription

            }).ToList();
               
        }

        private void button10_Click(object sender, EventArgs e)
        {
            //products tablosuna LimonluSu ekle UnitPrice = 55 Discontinued=false olarak ayarlansın
            //çalışanlarımdan Nancy'e ait olan bilgileri bul (Nancy'nin ID bilgisini gir)
            //Siparişle git Nancy tarafından ALFKI id'sine sahip Company'yi ekleyerek sipariş ekle.
            //Yukarıda eklediğin siparişin ID bilgisi bul sonra order_details tablosuna bu bilgileri kullanarak sipariş detayı ekle bu detayda ProductId =Yukarıda eklediğin LimonSuyun Id bilgisi gelecek, Unit Price=22,Quantity =3, Discount =0.25F olarak database'e ekle ve database'i kaydet

            Product product = new Product() { ProductName = "KavunluSu", Discontinued = false, UnitPrice = 55 };
            db.Products.Add(product);
            db.SaveChanges();
            db.SaveChangesAsync();

            var eklenenUrunID = db.Products.Where(x => x.ProductName == product.ProductName).Select(x => x.ProductID).FirstOrDefault();

            var nancyID = db.Employees.Where(x => x.FirstName == "Nancy").Select(x => x.EmployeeID).FirstOrDefault();

            db.Orders.Add(new Order() { CustomerID = "ALFKI", EmployeeID = nancyID });
            db.SaveChanges();

            var siralananSiparisler = db.Orders.OrderBy(x => x.OrderID).ToList();
            var sonSiparis = siralananSiparisler[siralananSiparisler.Count() - 1];

            db.Order_Details.Add(new Order_Detail() { OrderID = sonSiparis.OrderID, ProductID = eklenenUrunID, UnitPrice = 22, Quantity = 3, Discount = 0.25F });
            db.SaveChanges();
        }
    }
}
