using System;
using System.Collections.Generic;
using System.Linq;

namespace KutuphaneOtomasyonu
{
    #region 1. Enum ve Arayüz (Interface) Tanımları

    /// <summary>
    /// Ödünç alma işleminin güncel durumunu ifade eder.
    /// </summary>
    public enum OduncDurumu
    {
        DevamEdiyor,
        TeslimEdildi,
        Gecikmede,
        IptalEdildi
    }

    /// <summary>
    /// Kütüphane üyelik tipleri ve ödünç limitleri
    /// </summary>
    public enum UyelikTipi
    {
        Ogrenci,
        Akademisyen,
        IdariPersonel,
        Genel
    }

    /// <summary>
    /// Sistemdeki tüm insan varlıklarının temel sözleşmesi
    /// </summary>
    public interface IKisi
    {
        int Id { get; }
        string Ad { get; set; }
        string Soyad { get; set; }
        string Eposta { get; set; }
        string Telefon { get; set; }
        string TamAd();
        void BilgiYazdir();
    }

    /// <summary>
    /// Kitap ödünç alma ve iade işlemlerinin sözleşmesi
    /// </summary>
    public interface IOduncYonetimi
    {
        OduncKaydi KitapOduncVer(Uye uye, Kitap kitap, int oduncGunu = 15);
        decimal KitapTeslimAl(OduncKaydi oduncKaydi);
    }

    #endregion

    #region 2. Temel Varlıklar ve Kalıtım (Inheritance)

    /// <summary>
    /// Soyut Temel Sınıf (Abstract Base Class)
    /// OOP Prensipleri: Abstraction (Soyutlama) ve Encapsulation (Kapsülleme)
    /// </summary>
    public abstract class Kisi : IKisi
    {
        private string _ad;
        private string _soyad;
        private string _eposta;

        public int Id { get; protected set; }

        public string Ad
        {
            get => _ad;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Ad alanı boş bırakılamaz.");
                _ad = value.Trim();
            }
        }

        public string Soyad
        {
            get => _soyad;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Soyad alanı boş bırakılamaz.");
                _soyad = value.Trim();
            }
        }

        public string Eposta
        {
            get => _eposta;
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && !value.Contains("@"))
                    throw new ArgumentException("Geçerli bir e-posta adresi giriniz.");
                _eposta = value?.Trim();
            }
        }

        public string Telefon { get; set; }

        protected Kisi(int id, string ad, string soyad, string eposta, string telefon)
        {
            Id = id;
            Ad = ad;
            Soyad = soyad;
            Eposta = eposta;
            Telefon = telefon;
        }

        public virtual string TamAd() => $"{Ad} {Soyad}";

        public virtual void BilgiYazdir()
        {
            Console.WriteLine($"[ID: {Id}] {TamAd()} | E-posta: {Eposta} | Tel: {Telefon}");
        }
    }

    public class Uye : Kisi
    {
        public string UyeNo { get; private set; }
        public UyelikTipi Tip { get; set; }
        public DateTime UyelikTarihi { get; private set; }
        public bool AktifMi { get; set; }
        public decimal ToplamCezaBorcu { get; private set; }
        public List<OduncKaydi> AktifOduncler { get; private set; }

        public Uye(int id, string ad, string soyad, string eposta, string telefon, string uyeNo, UyelikTipi tip)
            : base(id, ad, soyad, eposta, telefon)
        {
            UyeNo = uyeNo;
            Tip = tip;
            UyelikTarihi = DateTime.Now;
            AktifMi = true;
            ToplamCezaBorcu = 0.0m;
            AktifOduncler = new List<OduncKaydi>();
        }

        public int MaksimumKitapLimiti()
        {
            return Tip switch
            {
                UyelikTipi.Akademisyen => 10,
                UyelikTipi.Ogrenci => 5,
                UyelikTipi.IdariPersonel => 4,
                _ => 3
            };
        }

        public bool OduncAlabilirMi()
        {
            return AktifMi && ToplamCezaBorcu <= 50.0m && AktifOduncler.Count < MaksimumKitapLimiti();
        }

        public void CezaEkle(decimal tutar)
        {
            if (tutar > 0)
                ToplamCezaBorcu += tutar;
        }

        public void CezaOde(decimal tutar)
        {
            if (tutar <= 0) return;
            ToplamCezaBorcu = Math.Max(0, ToplamCezaBorcu - tutar);
        }

        public override void BilgiYazdir()
        {
            base.BilgiYazdir();
            Console.WriteLine($"\t-> Üye No: {UyeNo} | Tip: {Tip} | Aktif Ödünç: {AktifOduncler.Count}/{MaksimumKitapLimiti()} | Borç: {ToplamCezaBorcu:C2}");
        }
    }

    public class Kutuphaneci : Kisi
    {
        public string PersonelKodu { get; private set; }
        public string Unvan { get; set; }
        public int YetkiSeviyesi { get; set; }

        public Kutuphaneci(int id, string ad, string soyad, string eposta, string telefon, string personelKodu, string unvan, int yetkiSeviyesi = 1)
            : base(id, ad, soyad, eposta, telefon)
        {
            PersonelKodu = personelKodu;
            Unvan = unvan;
            YetkiSeviyesi = yetkiSeviyesi;
        }

        public Kitap YeniKitapKaydet(string isbn, string baslik, string yazar, int yayinYili, int stok, Kategori kategori)
        {
            Console.WriteLine($"[Kütüphaneci {TamAd()}] Yeni kitap sisteme eklendi: '{baslik}'");
            return new Kitap(new Random().Next(1000, 9999), isbn, baslik, yazar, yayinYili, stok, kategori);
        }

        public override void BilgiYazdir()
        {
            base.BilgiYazdir();
            Console.WriteLine($"\t-> Personel Kodu: {PersonelKodu} | Unvan: {Unvan} | Yetki: Seviye {YetkiSeviyesi}");
        }
    }

    #endregion

    #region 3. Alan Modelleri: Kategori, Kitap ve Ödünç Kaydı

    public class Kategori
    {
        public int KategoriId { get; set; }
        public string Ad { get; set; }
        public string Aciklama { get; set; }

        public Kategori(int id, string ad, string aciklama = "")
        {
            KategoriId = id;
            Ad = ad;
            Aciklama = aciklama;
        }
    }

    public class Kitap
    {
        public int KitapId { get; private set; }
        public string ISBN { get; private set; }
        public string Baslik { get; set; }
        public string Yazar { get; set; }
        public int YayinYili { get; set; }
        public int StokAdedi { get; private set; }
        public Kategori Kategori { get; set; }

        public Kitap(int kitapId, string isbn, string baslik, string yazar, int yayinYili, int stokAdedi, Kategori kategori)
        {
            KitapId = kitapId;
            ISBN = isbn;
            Baslik = baslik;
            Yazar = yazar;
            YayinYili = yayinYili;
            StokAdedi = Math.Max(0, stokAdedi);
            Kategori = kategori;
        }

        public bool OduncVerilebilirMi() => StokAdedi > 0;

        public void StokAzalt()
        {
            if (StokAdedi <= 0)
                throw new InvalidOperationException($"'{Baslik}' adlı kitabın stok adedi yetersiz!");
            StokAdedi--;
        }

        public void StokArtir()
        {
            StokAdedi++;
        }

        public override string ToString() => $"[ISBN: {ISBN}] {Baslik} - {Yazar} ({YayinYili}) | Kategori: {Kategori?.Ad} | Stok: {StokAdedi}";
    }

    public class OduncKaydi
    {
        public Guid IslemId { get; private set; }
        public Uye Uye { get; private set; }
        public Kitap Kitap { get; private set; }
        public DateTime OduncTarihi { get; private set; }
        public DateTime SonTeslimTarihi { get; private set; }
        public DateTime? TeslimTarihi { get; private set; }
        public OduncDurumu Durum { get; private set; }
        public decimal CezaTutari { get; private set; }

        public const decimal GunlukGecikmeCezasi = 5.0m;

        public OduncKaydi(Uye uye, Kitap kitap, int gunSayisi = 15)
        {
            IslemId = Guid.NewGuid();
            Uye = uye ?? throw new ArgumentNullException(nameof(uye));
            Kitap = kitap ?? throw new ArgumentNullException(nameof(kitap));
            OduncTarihi = DateTime.Now;
            SonTeslimTarihi = OduncTarihi.AddDays(gunSayisi);
            Durum = OduncDurumu.DevamEdiyor;
            CezaTutari = 0.0m;
        }

        public int GecikmeGunSayisi(DateTime kontrolTarihi)
        {
            var kapanisTarihi = TeslimTarihi ?? kontrolTarihi;
            if (kapanisTarihi > SonTeslimTarihi)
            {
                return (int)Math.Ceiling((kapanisTarihi - SonTeslimTarihi).TotalDays);
            }
            return 0;
        }

        public decimal CezaHesapla(DateTime kontrolTarihi)
        {
            int gecikenGun = GecikmeGunSayisi(kontrolTarihi);
            CezaTutari = gecikenGun * GunlukGecikmeCezasi;
            return CezaTutari;
        }

        public void IslemiKapat(DateTime teslimTarihi)
        {
            TeslimTarihi = teslimTarihi;
            decimal ceza = CezaHesapla(teslimTarihi);
            if (ceza > 0)
            {
                Durum = OduncDurumu.Gecikmede;
                Uye.CezaEkle(ceza);
            }
            else
            {
                Durum = OduncDurumu.TeslimEdildi;
            }

            Kitap.StokArtir();
            Uye.AktifOduncler.Remove(this);
        }
    }

    #endregion

    #region 4. Kütüphane Yönetim Servisi (Facade / Service Pattern)

    public class KutuphaneYonetimi : IOduncYonetimi
    {
        public string KutuphaneAdi { get; set; }
        public List<Kitap> Envanter { get; private set; }
        public List<Uye> Uyeler { get; private set; }
        public List<Kutuphaneci> Personeller { get; private set; }
        public List<OduncKaydi> TumOduncKayitlari { get; private set; }

        public KutuphaneYonetimi(string kutuphaneAdi)
        {
            KutuphaneAdi = kutuphaneAdi;
            Envanter = new List<Kitap>();
            Uyeler = new List<Uye>();
            Personeller = new List<Kutuphaneci>();
            TumOduncKayitlari = new List<OduncKaydi>();
        }

        public OduncKaydi KitapOduncVer(Uye uye, Kitap kitap, int oduncGunu = 15)
        {
            if (uye == null) throw new ArgumentNullException(nameof(uye));
            if (kitap == null) throw new ArgumentNullException(nameof(kitap));

            if (!uye.OduncAlabilirMi())
            {
                throw new InvalidOperationException($"[HATA] Üye '{uye.TamAd()}' ödünç alamaz! Limit dolu veya borç bulunuyor.");
            }

            if (!kitap.OduncVerilebilirMi())
            {
                throw new InvalidOperationException($"[HATA] '{kitap.Baslik}' stokta bulunmamaktadır.");
            }

            kitap.StokAzalt();
            var kayit = new OduncKaydi(uye, kitap, oduncGunu);
            uye.AktifOduncler.Add(kayit);
            TumOduncKayitlari.Add(kayit);

            Console.WriteLine($"[BAŞARILI] '{kitap.Baslik}' kitabı '{uye.TamAd()}' adlı üyeye ödünç verildi. Son Teslim: {kayit.SonTeslimTarihi:dd.MM.yyyy}");
            return kayit;
        }

        public decimal KitapTeslimAl(OduncKaydi oduncKaydi)
        {
            if (oduncKaydi == null) throw new ArgumentNullException(nameof(oduncKaydi));

            DateTime bugun = DateTime.Now;
            oduncKaydi.IslemiKapat(bugun);

            Console.WriteLine($"[İADE EDİLDİ] '{oduncKaydi.Kitap.Baslik}' iade alındı. Hesaplanan Ceza: {oduncKaydi.CezaTutari:C2}");
            return oduncKaydi.CezaTutari;
        }
    }

    #endregion
}
