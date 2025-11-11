# Vendor Portal - React Frontend

## 🚀 React Frontend Kurulumu

### Gereksinimler
- Node.js 18+ ve npm
- .NET 9.0 SDK
- PostgreSQL

### Kurulum Adımları

#### 1. Backend'i Başlat

```bash
# VendorPortal ana dizininde
dotnet restore
dotnet ef database update
dotnet run
```

Backend şu adreste çalışacak: https://localhost:5001

#### 2. React Frontend'i Başlat

```bash
# ClientApp dizininde
cd ClientApp
npm install
npm start
```

React app şu adreste açılacak: http://localhost:3000

### 📦 React Yapısı

```
ClientApp/
├── public/
│   └── index.html
├── src/
│   ├── api/
│   │   └── apiClient.js          # API iletişim katmanı
│   ├── components/
│   │   ├── Auth/
│   │   │   └── PrivateRoute.js   # Route koruma
│   │   └── Layout/
│   │       └── Layout.js         # Ana layout
│   ├── context/
│   │   └── AuthContext.js        # Auth state yönetimi
│   ├── pages/
│   │   ├── Auth/
│   │   │   └── Login.js          # Login sayfası
│   │   ├── Dashboard/
│   │   │   └── Dashboard.js      # Dashboard
│   │   ├── Firma/
│   │   │   ├── FirmaList.js
│   │   │   ├── FirmaCreate.js
│   │   │   ├── FirmaEdit.js
│   │   │   └── FirmaDetails.js
│   │   ├── Malzeme/
│   │   │   └── ...
│   │   ├── Ihale/
│   │   │   └── ...
│   │   └── Siparis/
│   │       └── ...
│   ├── App.js                    # Ana component
│   ├── index.js                  # Giriş noktası
│   └── index.css                 # Global stil
├── package.json
└── README.md
```

### 🎯 Özellikler

- ✅ **React 18** - Modern React hooks
- ✅ **React Router v6** - Routing
- ✅ **React Bootstrap** - UI components
- ✅ **Axios** - API requests
- ✅ **JWT Authentication** - Token based auth
- ✅ **Context API** - State management
- ✅ **React Toastify** - Notifications
- ✅ **FontAwesome** - Icons

### 📡 API Entegrasyonu

API client (`src/api/apiClient.js`) otomatik olarak:
- JWT token ekler
- Hata yönetimi yapar
- Toast bildirimleri gösterir

```javascript
// Örnek kullanım
import { firmaApi } from '../api/apiClient';

const firmalar = await firmaApi.getAll();
```

### 🔐 Authentication

Context API ile yönetilir:

```javascript
import { useAuth } from '../context/AuthContext';

const MyComponent = () => {
  const { user, isAuthenticated, login, logout, hasRole } = useAuth();
  
  return (
    <div>
      {hasRole('Admin') && <AdminPanel />}
    </div>
  );
};
```

### 🎨 Component'ler

#### Tamamlanan:
- ✅ Layout
- ✅ PrivateRoute
- ✅ Login
- ✅ Dashboard
- ✅ FirmaList
- ✅ FirmaCreate

#### Placeholder (Geliştirmeye Hazır):
- ⏳ FirmaEdit
- ⏳ FirmaDetails
- ⏳ Malzeme modülü
- ⏳ İhale modülü
- ⏳ Sipariş modülü

### 📝 Notlar

- Proxy ayarı `package.json`'da tanımlı: Backend'e otomatik yönlendirir
- CORS ayarları backend'de yapılandırılmalı
- Production build: `npm run build`

### 🔧 Geliştirme

Yeni sayfa eklemek için:

1. Component oluştur: `src/pages/ModulAdi/SayfaAdi.js`
2. Route ekle: `src/App.js`
3. Navigation ekle: `src/components/Layout/Layout.js`
4. API method ekle: `src/api/apiClient.js`

### 🚢 Production Build

```bash
npm run build
```

Build klasörü `ClientApp/build/` içinde oluşur.

---

**Not:** Backend ve Frontend ayrı portlarda çalışır. Development'ta proxy kullanılır, production'da Nginx/IIS ile reverse proxy yapılmalı.
