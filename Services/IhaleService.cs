using Microsoft.EntityFrameworkCore;
using VendorPortal.Data;
using VendorPortal.Models;
using VendorPortal.Models.Entities;
using VendorPortal.Models.Enums;

namespace VendorPortal.Services
{
    /// <summary>
    /// İhale servisi implementasyonu
    /// </summary>
    public class IhaleService : IIhaleService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<IhaleService> _logger;

        public IhaleService(ApplicationDbContext context, ILogger<IhaleService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ServiceResult<Ihale>> CreateIhaleAsync(Ihale ihale, List<IhaleKalem> kalemler, Guid userId)
        {
            try
            {
                ihale.Id = Guid.NewGuid();
                ihale.CreatedDate = DateTime.UtcNow;
                ihale.CreatedByUserId = userId;
                ihale.Durum = IhaleDurumu.Taslak;

                // Kalemler ekle
                foreach (var kalem in kalemler)
                {
                    kalem.Id = Guid.NewGuid();
                    kalem.IhaleId = ihale.Id;
                    kalem.CreatedDate = DateTime.UtcNow;
                    kalem.CreatedByUserId = userId;
                    ihale.Kalemler.Add(kalem);
                }

                _context.Ihaleler.Add(ihale);
                await _context.SaveChangesAsync();

                return ServiceResult<Ihale>.SuccessResult(ihale, "İhale başarıyla oluşturuldu.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İhale oluşturulurken hata");
                return ServiceResult<Ihale>.ErrorResult("İhale oluşturulurken hata oluştu: " + ex.Message);
            }
        }

        public async Task<ServiceResult<Ihale>> UpdateIhaleAsync(Ihale ihale, Guid userId)
        {
            try
            {
                var existing = await _context.Ihaleler.FindAsync(ihale.Id);
                if (existing == null || existing.IsDeleted)
                {
                    return ServiceResult<Ihale>.ErrorResult("İhale bulunamadı.");
                }

                if (existing.Durum != IhaleDurumu.Taslak)
                {
                    return ServiceResult<Ihale>.ErrorResult("Sadece taslak ihaleler güncellenebilir.");
                }

                existing.IhaleAdi = ihale.IhaleAdi;
                existing.IhaleTuru = ihale.IhaleTuru;
                existing.Aciklama = ihale.Aciklama;
                existing.TeklifBaslangicTarihi = ihale.TeklifBaslangicTarihi;
                existing.TeklifBitisTarihi = ihale.TeklifBitisTarihi;
                existing.GecerlilikSuresiGun = ihale.GecerlilikSuresiGun;
                existing.ParaBirimi = ihale.ParaBirimi;
                existing.HerkeseAcikMi = ihale.HerkeseAcikMi;
                existing.TumTedarikcilereAcikMi = ihale.TumTedarikcilereAcikMi;

                await _context.SaveChangesAsync();

                return ServiceResult<Ihale>.SuccessResult(existing, "İhale başarıyla güncellendi.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İhale güncellenirken hata");
                return ServiceResult<Ihale>.ErrorResult("İhale güncellenirken hata oluştu: " + ex.Message);
            }
        }

        public async Task<ServiceResult<Ihale>> GetIhaleByIdAsync(Guid id)
        {
            try
            {
                var ihale = await _context.Ihaleler
                    .Include(i => i.MusteriFirma)
                    .Include(i => i.Kalemler)
                        .ThenInclude(k => k.Malzeme)
                    .Include(i => i.Davetler)
                        .ThenInclude(d => d.TedarikciFirma)
                    .Include(i => i.Teklifler)
                        .ThenInclude(t => t.TedarikciFirma)
                    .FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted);

                if (ihale == null)
                {
                    return ServiceResult<Ihale>.ErrorResult("İhale bulunamadı.");
                }

                return ServiceResult<Ihale>.SuccessResult(ihale);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İhale getirilirken hata");
                return ServiceResult<Ihale>.ErrorResult("İhale getirilirken hata oluştu: " + ex.Message);
            }
        }

        public async Task<ServiceResult<List<Ihale>>> GetIhalelerByFirmaAsync(Guid firmaId)
        {
            try
            {
                var ihaleler = await _context.Ihaleler
                    .Where(i => i.MusteriFirmaId == firmaId && !i.IsDeleted)
                    .Include(i => i.MusteriFirma)
                    .Include(i => i.Kalemler)
                    .OrderByDescending(i => i.CreatedDate)
                    .ToListAsync();

                return ServiceResult<List<Ihale>>.SuccessResult(ihaleler);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İhaleler listelenirken hata");
                return ServiceResult<List<Ihale>>.ErrorResult("İhaleler listelenirken hata oluştu: " + ex.Message);
            }
        }

        public async Task<ServiceResult<List<Ihale>>> GetAllIhalelerAsync()
        {
            try
            {
                var ihaleler = await _context.Ihaleler
                    .Where(i => !i.IsDeleted)
                    .Include(i => i.MusteriFirma)
                    .Include(i => i.Kalemler)
                    .OrderByDescending(i => i.CreatedDate)
                    .ToListAsync();

                return ServiceResult<List<Ihale>>.SuccessResult(ihaleler);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İhaleler listelenirken hata");
                return ServiceResult<List<Ihale>>.ErrorResult("İhaleler listelenirken hata oluştu: " + ex.Message);
            }
        }

        public async Task<ServiceResult> DeleteIhaleAsync(Guid id, Guid userId)
        {
            try
            {
                var ihale = await _context.Ihaleler.FindAsync(id);
                if (ihale == null)
                {
                    return ServiceResult.ErrorResult("İhale bulunamadı.");
                }

                if (ihale.Durum != IhaleDurumu.Taslak)
                {
                    return ServiceResult.ErrorResult("Sadece taslak ihaleler silinebilir.");
                }

                ihale.IsDeleted = true;

                await _context.SaveChangesAsync();

                return ServiceResult.SuccessResult("İhale başarıyla silindi.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İhale silinirken hata");
                return ServiceResult.ErrorResult("İhale silinirken hata oluştu: " + ex.Message);
            }
        }

        public async Task<ServiceResult> YayinlaIhaleAsync(Guid id, Guid userId)
        {
            try
            {
                var ihale = await _context.Ihaleler
                    .Include(i => i.Kalemler)
                    .FirstOrDefaultAsync(i => i.Id == id);

                if (ihale == null || ihale.IsDeleted)
                {
                    return ServiceResult.ErrorResult("İhale bulunamadı.");
                }

                if (ihale.Durum != IhaleDurumu.Taslak)
                {
                    return ServiceResult.ErrorResult("Sadece taslak ihaleler yayınlanabilir.");
                }

                if (!ihale.Kalemler.Any())
                {
                    return ServiceResult.ErrorResult("İhale en az bir kalem içermelidir.");
                }

                ihale.Durum = IhaleDurumu.Yayinda;

                await _context.SaveChangesAsync();

                return ServiceResult.SuccessResult("İhale başarıyla yayınlandı.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İhale yayınlanırken hata");
                return ServiceResult.ErrorResult("İhale yayınlanırken hata oluştu: " + ex.Message);
            }
        }

        public async Task<ServiceResult> IptalEtIhaleAsync(Guid id, string iptalNedeni, Guid userId)
        {
            try
            {
                var ihale = await _context.Ihaleler.FindAsync(id);
                if (ihale == null || ihale.IsDeleted)
                {
                    return ServiceResult.ErrorResult("İhale bulunamadı.");
                }

                ihale.Durum = IhaleDurumu.IptalEdildi;

                await _context.SaveChangesAsync();

                return ServiceResult.SuccessResult("İhale başarıyla iptal edildi.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İhale iptal edilirken hata");
                return ServiceResult.ErrorResult("İhale iptal edilirken hata oluştu: " + ex.Message);
            }
        }

        public async Task<ServiceResult<IhaleDaveti>> CreateDavetAsync(Guid ihaleId, Guid tedarikciFirmaId, Guid userId)
        {
            try
            {
                // Mevcut davet var mı kontrol et
                var mevcutDavet = await _context.IhaleDavetleri
                    .FirstOrDefaultAsync(d => d.IhaleId == ihaleId && 
                                             d.TedarikciFirmaId == tedarikciFirmaId);

                if (mevcutDavet != null)
                {
                    return ServiceResult<IhaleDaveti>.ErrorResult("Bu tedarikçi zaten davet edilmiş.");
                }

                var davet = new IhaleDaveti
                {
                    Id = Guid.NewGuid(),
                    IhaleId = ihaleId,
                    TedarikciFirmaId = tedarikciFirmaId,
                    DavetTarihi = DateTime.UtcNow,
                    CreatedDate = DateTime.UtcNow,
                    CreatedByUserId = userId
                };

                _context.IhaleDavetleri.Add(davet);
                await _context.SaveChangesAsync();

                return ServiceResult<IhaleDaveti>.SuccessResult(davet, "Davet başarıyla oluşturuldu.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Davet oluşturulurken hata");
                return ServiceResult<IhaleDaveti>.ErrorResult("Davet oluşturulurken hata oluştu: " + ex.Message);
            }
        }

        public async Task<ServiceResult<IhaleTeklif>> CreateTeklifAsync(
            IhaleTeklif teklif, List<IhaleTeklifKalem> kalemler, Guid userId)
        {
            try
            {
                teklif.Id = Guid.NewGuid();
                teklif.CreatedDate = DateTime.UtcNow;
                teklif.CreatedByUserId = userId;
                teklif.Durum = TeklifDurumu.Taslak;

                // Kalemler ekle ve toplam tutarı hesapla
                decimal toplamTutar = 0;
                foreach (var kalem in kalemler)
                {
                    kalem.Id = Guid.NewGuid();
                    kalem.IhaleTeklifId = teklif.Id;
                    kalem.CreatedDate = DateTime.UtcNow;
                    kalem.CreatedByUserId = userId;
                    kalem.ToplamFiyat = kalem.TeklifEdilenMiktar * kalem.BirimFiyat;
                    toplamTutar += kalem.ToplamFiyat;
                    teklif.TeklifKalemleri.Add(kalem);
                }

                teklif.ToplamTeklifTutari = toplamTutar;

                _context.IhaleTeklifleri.Add(teklif);
                await _context.SaveChangesAsync();

                return ServiceResult<IhaleTeklif>.SuccessResult(teklif, "Teklif başarıyla oluşturuldu.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Teklif oluşturulurken hata");
                return ServiceResult<IhaleTeklif>.ErrorResult("Teklif oluşturulurken hata oluştu: " + ex.Message);
            }
        }

        public async Task<ServiceResult<List<IhaleTeklif>>> GetTekliflerByIhaleAsync(Guid ihaleId)
        {
            try
            {
                var teklifler = await _context.IhaleTeklifleri
                    .Where(t => t.IhaleId == ihaleId && !t.IsDeleted)
                    .Include(t => t.TedarikciFirma)
                    .Include(t => t.TeklifKalemleri)
                        .ThenInclude(tk => tk.IhaleKalem)
                    .OrderByDescending(t => t.TeklifTarihi)
                    .ToListAsync();

                return ServiceResult<List<IhaleTeklif>>.SuccessResult(teklifler);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Teklifler getirilirken hata");
                return ServiceResult<List<IhaleTeklif>>.ErrorResult("Teklifler getirilirken hata oluştu: " + ex.Message);
            }
        }

        public async Task<ServiceResult> OnaylaTeklifAsync(Guid teklifId, Guid userId)
        {
            try
            {
                var teklif = await _context.IhaleTeklifleri.FindAsync(teklifId);
                if (teklif == null || teklif.IsDeleted)
                {
                    return ServiceResult.ErrorResult("Teklif bulunamadı.");
                }

                teklif.Durum = TeklifDurumu.Onaylandi;

                await _context.SaveChangesAsync();

                return ServiceResult.SuccessResult("Teklif başarıyla onaylandı.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Teklif onaylanırken hata");
                return ServiceResult.ErrorResult("Teklif onaylanırken hata oluştu: " + ex.Message);
            }
        }
    }
}
