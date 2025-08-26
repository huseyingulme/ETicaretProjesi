# E-Ticaret Projesi

Modern, ölçeklenebilir ve profesyonel bir e-ticaret web uygulaması. ASP.NET Core MVC, Entity Framework Core, Repository Pattern ve çok katmanlı mimari kullanılarak geliştirilmiştir.

## 🚀 Özellikler

### Kullanıcı Özellikleri

- **Kullanıcı Kayıt/Giriş Sistemi**: Güvenli kimlik doğrulama ve şifre sıfırlama
- **Ürün Kataloğu**: Kategorilere göre ürün listeleme ve arama
- **Sepet Yönetimi**: Ürün ekleme/çıkarma, miktar güncelleme
- **Favoriler**: Beğenilen ürünleri kaydetme
- **Adres Yönetimi**: Çoklu adres ekleme/düzenleme
- **Sipariş Takibi**: Sipariş geçmişi ve durumu
- **İletişim Formu**: Müşteri destek sistemi
- **Ürün Arama**: Gelişmiş arama ve filtreleme
- **Fiyat Aralığı**: Fiyat bazlı ürün filtreleme

### Admin Özellikleri

- **Ürün Yönetimi**: Ürün ekleme, düzenleme, silme
- **Kategori Yönetimi**: Kategori CRUD işlemleri
- **Marka Yönetimi**: Marka ekleme ve düzenleme
- **Slider Yönetimi**: Ana sayfa slider'ları
- **Sipariş Yönetimi**: Sipariş durumu güncelleme
- **Kullanıcı Yönetimi**: Kullanıcı hesapları yönetimi
- **İstatistikler**: Detaylı satış ve ürün istatistikleri

### Yeni Eklenen Özellikler

- **Repository Pattern**: Generic ve specific repository implementasyonu
- **Business Logic Layer**: Servis katmanı ile iş mantığı ayrımı
- **Caching System**: Memory cache ile performans optimizasyonu
- **Email Service**: Otomatik email gönderimi (hoş geldin, sipariş onayı, şifre sıfırlama)
- **Validation**: FluentValidation ile güçlü veri doğrulama
- **API Endpoints**: RESTful API desteği
- **Extension Methods**: String ve DateTime utility fonksiyonları
- **Configuration Management**: Merkezi ayar yönetimi
- **Password Security**: Güçlü şifre hash'leme ve doğrulama
- **File Management**: Güvenli dosya yükleme ve yönetimi

## 🛠️ Teknolojiler

### Backend

- **Framework**: ASP.NET Core 8.0 MVC
- **Veritabanı**: SQL Server (LocalDB)
- **ORM**: Entity Framework Core 9.0
- **Validation**: FluentValidation 11.8.1
- **Caching**: Memory Cache
- **Architecture**: Repository Pattern, Service Layer, Dependency Injection

### Frontend

- **HTML5, CSS3, JavaScript**
- **Bootstrap**: Responsive tasarım
- **Razor Views**: Server-side rendering

### Güvenlik

- **Authentication**: Cookie Authentication
- **Password Hashing**: PBKDF2 with SHA256
- **Data Validation**: FluentValidation
- **File Security**: File type and size validation

### API

- **RESTful API**: Product management endpoints
- **JSON**: Data serialization
- **Error Handling**: Comprehensive error management

## 📁 Proje Yapısı

```
ETicaret/
├── ETicaret.Core/                    # Core katmanı
│   ├── Entities/                     # Veritabanı entity'leri
│   ├── Models/                       # View modelleri ve DTO'lar
│   ├── Interfaces/                   # Repository ve servis interface'leri
│   ├── Services/                     # Servis interface'leri
│   ├── Extensions/                   # Extension method'lar
│   ├── Validators/                   # FluentValidation validator'ları
│   ├── Helpers/                      # Yardımcı sınıflar
│   └── Configuration/                # Konfigürasyon sınıfları
├── ETicaret.Data/                    # Veri erişim katmanı
│   ├── Configurations/               # Entity konfigürasyonları
│   ├── Repositories/                 # Repository implementasyonları
│   ├── Migrations/                   # EF Core migration'ları
│   └── DatabaseContext.cs            # DbContext
├── ETicaret/                         # Ana web uygulaması
│   ├── Areas/Admin/                  # Admin paneli
│   ├── Controllers/                  # MVC Controller'ları
│   │   └── Api/                      # API Controller'ları
│   ├── Services/                     # Servis implementasyonları
│   ├── Views/                        # Razor view'ları
│   ├── wwwroot/                      # Statik dosyalar
│   └── Program.cs                    # Uygulama giriş noktası
└── README.md
```

### Katman Mimarisi

- **ETicaret.Core**: İş mantığı, entity'ler, interface'ler ve yardımcı sınıflar
- **ETicaret.Data**: Veri erişim katmanı, repository pattern implementasyonu
- **ETicaret**: Presentation katmanı, MVC controller'lar ve view'lar

## 🚀 Kurulum

### Gereksinimler

- .NET 8.0 SDK
- SQL Server LocalDB (Visual Studio ile birlikte gelir)
- Visual Studio 2022 veya VS Code
- Git

### Adım 1: Projeyi Klonlayın

```bash
git clone https://github.com/kullaniciadi/ETicaret.git
cd ETicaret
```

### Adım 2: Paketleri Yükleyin

```bash
# Tüm projeler için paketleri yükleyin
dotnet restore
```

### Adım 3: Veritabanını Oluşturun

```bash
# ETicaret klasörüne gidin
cd ETicaret

# Migration'ları uygulayın
dotnet ef database update
```

### Adım 4: Uygulamayı Çalıştırın

```bash
dotnet run
```

Uygulama `https://localhost:5001` adresinde çalışacaktır.

### Adım 5: Admin Kullanıcısı Oluşturun

Veritabanı oluşturulduktan sonra, admin paneline erişim için admin kullanıcısı oluşturmanız gerekmektedir. Bunu veritabanına manuel olarak ekleyebilir veya seed data ile otomatik oluşturabilirsiniz.

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
