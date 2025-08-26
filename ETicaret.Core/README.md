# E-Ticaret Projesi

Modern bir e-ticaret web uygulaması. ASP.NET Core MVC, Entity Framework Core ve SQL Server kullanılarak geliştirilmiştir.

## 🚀 Özellikler

### Kullanıcı Özellikleri

- **Kullanıcı Kayıt/Giriş Sistemi**: Güvenli kimlik doğrulama
- **Ürün Kataloğu**: Kategorilere göre ürün listeleme
- **Sepet Yönetimi**: Ürün ekleme/çıkarma, miktar güncelleme
- **Favoriler**: Beğenilen ürünleri kaydetme
- **Adres Yönetimi**: Çoklu adres ekleme/düzenleme
- **Sipariş Takibi**: Sipariş geçmişi ve durumu
- **İletişim Formu**: Müşteri destek sistemi

### Admin Özellikleri

- **Ürün Yönetimi**: Ürün ekleme, düzenleme, silme
- **Kategori Yönetimi**: Kategori CRUD işlemleri
- **Marka Yönetimi**: Marka ekleme ve düzenleme
- **Slider Yönetimi**: Ana sayfa slider'ları
- **Sipariş Yönetimi**: Sipariş durumu güncelleme
- **Kullanıcı Yönetimi**: Kullanıcı hesapları yönetimi

## 🛠️ Teknolojiler

- **Backend**: ASP.NET Core 8.0 MVC
- **Veritabanı**: SQL Server (LocalDB)
- **ORM**: Entity Framework Core 9.0
- **Frontend**: HTML5, CSS3, JavaScript, Bootstrap
- **Kimlik Doğrulama**: Cookie Authentication
- **Session Yönetimi**: ASP.NET Core Session

## 📁 Proje Yapısı

```
ETicaret/
├── ETicaret.Core/          # Entity'ler ve modeller
│   ├── Entities/           # Veritabanı entity'leri
│   └── Models/             # View modelleri
├── ETicaret.Data/          # Veritabanı katmanı
│   ├── Configurations/     # Entity konfigürasyonları
│   └── Migrations/         # EF Core migration'ları
├── ETicaret/               # Ana web uygulaması
│   ├── Areas/Admin/        # Admin paneli
│   ├── Controllers/        # MVC Controller'ları
│   ├── Services/           # İş mantığı servisleri
│   ├── Views/              # Razor view'ları
│   └── wwwroot/            # Statik dosyalar
└── README.md
```

## 🚀 Kurulum

### Gereksinimler

- .NET 8.0 SDK
- SQL Server LocalDB (Visual Studio ile birlikte gelir)
- Visual Studio 2022 veya VS Code

### Adım 1: Projeyi Klonlayın

```bash
git clone https://github.com/kullaniciadi/ETicaret.git
cd ETicaret
```

### Adım 2: Veritabanını Oluşturun

```bash
# ETicaret klasörüne gidin
cd ETicaret

# Migration'ları uygulayın
dotnet ef database update
```

### Adım 3: Uygulamayı Çalıştırın

```bash
dotnet run
```

Uygulama `https://localhost:5001` adresinde çalışacaktır.

## 🗄️ Veritabanı Yapısı

### Ana Tablolar

- **AppUsers**: Kullanıcı bilgileri
- **Products**: Ürün bilgileri
- **Categories**: Ürün kategorileri
- **Brands**: Ürün markaları
- **Carts**: Sepet bilgileri
- **CartItems**: Sepet ürünleri
- **Orders**: Sipariş bilgileri
- **OrderItems**: Sipariş ürünleri
- **Addresses**: Kullanıcı adresleri
- **Favorites**: Favori ürünler
- **Sliders**: Ana sayfa slider'ları
- **Contacts**: İletişim mesajları

### Veritabanı Bağlantısı

Varsayılan bağlantı dizesi `appsettings.json` dosyasında tanımlıdır:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=E-TicaretDb;Trusted_Connection=True;"
  }
}
```

## 🔧 Konfigürasyon

### Veritabanı Bağlantısını Değiştirme

`ETicaret/appsettings.json` dosyasındaki `ConnectionStrings` bölümünü düzenleyin:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=E-TicaretDb;User Id=YOUR_USER;Password=YOUR_PASSWORD;"
  }
}
```

### Admin Paneli

Admin paneline erişim için `/Admin` URL'ini kullanın. Varsayılan admin kullanıcısı oluşturmak için migration'ları çalıştırdıktan sonra veritabanına manuel olarak admin kullanıcısı ekleyebilirsiniz.

## 📱 Kullanım

### Kullanıcı Tarafı

1. Ana sayfada ürünleri görüntüleyin
2. Ürün detaylarına tıklayarak inceleyin
3. Sepete ürün ekleyin
4. Hesap oluşturun veya giriş yapın
5. Sipariş verin

### Admin Tarafı

1. `/Admin` URL'ine gidin
2. Admin paneline giriş yapın
3. Ürün, kategori, marka yönetimi yapın
4. Siparişleri takip edin
