import axios from 'axios';
import { toast } from 'react-toastify';

const API_BASE_URL = process.env.REACT_APP_API_URL || '/api';

const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json'
  }
});

// Request interceptor - Token ekle
apiClient.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('vendorPortalToken');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Response interceptor - Hata yönetimi
apiClient.interceptors.response.use(
  (response) => {
    return response;
  },
  (error) => {
    if (error.response) {
      // Server yanıt verdi ama hata kodu döndü
      const { status, data } = error.response;

      if (status === 401) {
        // Unauthorized - Token geçersiz veya yok
        localStorage.removeItem('vendorPortalToken');
        window.location.href = '/login';
        toast.error('Oturumunuz sona erdi. Lütfen tekrar giriş yapın.');
      } else if (status === 403) {
        toast.error('Bu işlem için yetkiniz bulunmamaktadır.');
      } else if (status === 404) {
        toast.error('İstenen kaynak bulunamadı.');
      } else if (status === 500) {
        toast.error('Sunucu hatası oluştu. Lütfen daha sonra tekrar deneyin.');
      } else {
        toast.error(data?.message || 'Bir hata oluştu.');
      }
    } else if (error.request) {
      // İstek gönderildi ama yanıt alınamadı
      toast.error('Sunucuya bağlanılamadı. İnternet bağlantınızı kontrol edin.');
    } else {
      // Başka bir hata
      toast.error('Beklenmeyen bir hata oluştu.');
    }

    return Promise.reject(error);
  }
);

// API Methods
export const authApi = {
  login: (credentials) => apiClient.post('/AccountApi/Login', credentials)
};

export const firmaApi = {
  getAll: () => apiClient.get('/FirmaApi/GetAll'),
  getById: (id) => apiClient.get(`/FirmaApi/Get/${id}`),
  create: (data) => apiClient.post('/FirmaApi/Create', data),
  update: (data) => apiClient.put('/FirmaApi/Update', data),
  delete: (id) => apiClient.delete(`/FirmaApi/Delete/${id}`),
  onayla: (id) => apiClient.post(`/FirmaApi/Onayla/${id}`),
  getByTip: (tip) => apiClient.get(`/FirmaApi/GetByTip/${tip}`),
  getTedarikciler: (musteriId) => apiClient.get(`/FirmaApi/GetTedarikciler/${musteriId}`)
};

export const malzemeApi = {
  getAll: () => apiClient.get('/MalzemeApi/GetAll'),
  getById: (id) => apiClient.get(`/MalzemeApi/Get/${id}`),
  getByFirma: (firmaId) => apiClient.get(`/MalzemeApi/GetByFirma/${firmaId}`),
  create: (data) => apiClient.post('/MalzemeApi/Create', data),
  update: (data) => apiClient.put('/MalzemeApi/Update', data),
  delete: (id) => apiClient.delete(`/MalzemeApi/Delete/${id}`),
  createEslestirme: (data) => apiClient.post('/MalzemeApi/CreateEslestirme', data),
  getEslestirmeler: (musteriId, tedarikciId) => 
    apiClient.get(`/MalzemeApi/GetEslestirmeler?musteriId=${musteriId}&tedarikciId=${tedarikciId}`)
};

export const ihaleApi = {
  getAll: () => apiClient.get('/IhaleApi/GetAll'),
  getById: (id) => apiClient.get(`/IhaleApi/Get/${id}`),
  getByFirma: (firmaId) => apiClient.get(`/IhaleApi/GetByFirma/${firmaId}`),
  create: (data) => apiClient.post('/IhaleApi/Create', data),
  update: (data) => apiClient.put('/IhaleApi/Update', data),
  delete: (id) => apiClient.delete(`/IhaleApi/Delete/${id}`),
  yayinla: (id) => apiClient.post(`/IhaleApi/Yayinla/${id}`),
  iptalEt: (id, iptalNedeni) => apiClient.post(`/IhaleApi/IptalEt/${id}`, { iptalNedeni }),
  davetEt: (data) => apiClient.post('/IhaleApi/DavetEt', data),
  teklifVer: (data) => apiClient.post('/IhaleApi/TeklifVer', data),
  getTeklifler: (ihaleId) => apiClient.get(`/IhaleApi/GetTeklifler/${ihaleId}`),
  onaylaTeklif: (teklifId) => apiClient.post(`/IhaleApi/OnaylaTeklif/${teklifId}`)
};

export const siparisApi = {
  postSiparisler: (data) => apiClient.post('/SiparisApi/PostSiparisler', data),
  createSiparis: (data) => apiClient.post('/SiparisApi/CreateSiparis', data),
  getSiparis: (id) => apiClient.get(`/SiparisApi/GetSiparis/${id}`),
  getSiparislerByFirma: (firmaId, isMusteriView = true) => 
    apiClient.get(`/SiparisApi/GetSiparislerByFirma/${firmaId}?isMusteriView=${isMusteriView}`)
};

export default apiClient;
