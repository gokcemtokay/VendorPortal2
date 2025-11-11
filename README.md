# Vendor Portal - Tedarik Zinciri Yönetim Sistemi

## 📋 Proje Hakkında

Vendor Portal, firmalar arası satın alma, ihale, sipariş ve doküman yönetimi süreçlerini uçtan uca dijitalleştiren, rol bazlı erişime sahip, modern ve kullanıcı dostu bir web uygulamasıdır.

## 🚀 Teknolojiler

- **Backend:** ASP.NET Core 9.0 MVC
- **Frontend:** Bootstrap 5, jQuery, Font Awesome
- **Veritabanı:** PostgreSQL
- **ORM:** Entity Framework Core (Code-First)
- **Authentication:** JWT + ASP.NET Core Identity
- **API Documentation:** Swagger/OpenAPI

## 📦 Kurulum

### Gereksinimler

- .NET 9.0 SDK
- PostgreSQL 16 veya üzeri
- Visual Studio 2022 veya VS Code

### Adımlar

1. **Projeyi klonlayın veya indirin**

2. **appsettings.json dosyasını düzenleyin**
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Port=5432;Database=VendorPortalDb;Username=postgres;Password=yourpassword"
     }
   }
   ```

3. **Veritabanını oluşturun**
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

4. **Projeyi çalıştırın**
   ```bash
   dotnet run
   ```

5. **Tarayıcınızda açın**
   - Web Arayüzü: https://localhost:5001
   - Swagger API: https://localhost:5001/swagger

## 👤 Varsayılan Kullanıcılar

### Admin Hesabı
- **Email:** admin@vendorportal.com
- **Şifre:** Admin123!

## 📚 API Kullanımı

### JWT Token Alma

```bash
POST /api/AccountApi/Login
Content-Type: application/json

{
  "email": "admin@vendorportal.com",
  "password": "Admin123!"
}
```

### Sipariş Oluşturma

```bash
POST /api/SiparisApi/PostSiparisler
Authorization: Bearer {token}
Content-Type: application/json

{
  "siparisler": [
    {
      "siparisNo": "00000001",
      "siparisTarihi": "2025-01-01T00:00:00",
      "musteriVergiNo": "1234567890",
      "tedarikciVergiNo": "9876543210",
      "paraBirimi": "TRY",
      "siparisTipi": 1,
      "teslimatAdresi": "İstanbul, Türkiye",
      "kayitTarihi": "2025-01-01T10:00:00",
      "kalemler": [
        {
          "kalemNo": 10,
          "malzemeNum": "MLZ001",
          "miktar": 100,
          "fiyat": 50.00,
          "indirimTutari": 0,
          "istenenTeslimTarihi": "2025-02-01T00:00:00",
          "kayitTarihi": "2025-01-01T10:00:00"
        }
      ]
    }
  ]
}
```

## 🔑 Roller ve Yetkiler

### Admin
- Tüm sistem verilerine erişim
- Tüm firmaların siparişlerini görüntüleme
- Sistem ayarlarını yönetme

### Müşteri
- Kendi firmasının satın alma süreçlerini yönetme
- Tedarikçilere sipariş oluşturma
- İhale açma ve yönetme

### Tedarikçi
- Kendi satış süreçlerini yönetme
- Müşterilerden gelen siparişleri takip etme
- İhalelere teklif verme

## 📁 Proje Yapısı

```
VendorPortal/
├── Areas/
│   └── Api/
│       └── Controllers/          # API Controller'ları
├── Controllers/                  # MVC Controller'ları
├── Data/                         # DbContext
├── Models/
│   ├── Entities/                 # Entity sınıfları
│   ├── Enums/                    # Enum tanımları
│   ├── DTOs/                     # Data Transfer Objects
│   └── ViewModels/               # View Model'leri
├── Services/                     # Business Logic
├── Workers/                      # Background Services
├── Views/                        # Razor Views
└── wwwroot/                      # Static files
```

## 🔧 Modüller

### Sipariş Yönetimi (✅ Tamamlandı)
- Sipariş oluşturma ve takip
- Kalem bazında onay/revize
- Durum geçmişi
- Mail bildirimleri
- API + MVC Controller'lar
- Service Layer
- View'lar (Index, Details)

### İhale Yönetimi (✅ Tamamlandı)
- İhale oluşturma (Açık, Kapalı, Davetli)
- Teklif toplama
- Teklif karşılaştırma matrisi
- Tedarikçi daveti
- API + MVC Controller'lar
- Service Layer
- View'lar (Index, Details, Create, Edit, Teklifler)

### Firma Yönetimi (✅ Tamamlandı)
- Müşteri-Tedarikçi ilişkileri
- Firma başvuruları
- Çoklu müşteri desteği
- API + MVC Controller'lar
- Service Layer
- View'lar (Index, Details, Create, Edit)

### Malzeme Yönetimi (✅ Tamamlandı)
- Malzeme kataloğu
- Fiyat yönetimi
- Müşteri-Tedarikçi malzeme eşleştirme
- API + MVC Controller'lar
- Service Layer
- View'lar (Index, Details, Create, Edit)

### Doküman Yönetimi
- Doküman yükleme ve saklama
- Versiyonlama
- Kategori bazlı organizasyon

## 🛠️ Geliştirme

### Migration Oluşturma
```bash
dotnet ef migrations add MigrationName
```

### Veritabanını Güncelleme
```bash
dotnet ef database update
```

### Seed Data Ekleme
Program.cs dosyasındaki `SeedData` metodunu düzenleyin.

## 📝 Notlar

- Geliştirme ortamında Swagger otomatik olarak aktiftir
- Production ortamında HTTPS zorunludur
- JWT token'lar 60 dakika geçerlidir
- Veritabanı bağlantı bilgilerini production'da environment variables ile yönetin

## 🔒 Güvenlik

- Şifreler hash'lenerek saklanır
- JWT token'lar imzalanır
- Rol bazlı yetkilendirme
- HTTPS zorunlu (production)
- CORS politikaları

## 📞 Destek

Sorularınız için: portal@ardenyazilim.com

## 📄 Lisans

Bu proje Arden Yazılım için özel olarak geliştirilmiştir.

---

**Geliştirici:** Claude AI Assistant  
**Versiyon:** 1.0.0  
**Son Güncelleme:** 11 Kasım 2025
