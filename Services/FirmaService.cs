using Microsoft.EntityFrameworkCore;
using VendorPortal.Data;
using VendorPortal.Models;
using VendorPortal.Models.Entities;
using VendorPortal.Models.Enums;

namespace VendorPortal.Services
{
    /// <summary>
    /// Firma servisi implementasyonu
    /// </summary>
    public class FirmaService : IFirmaService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<FirmaService> _logger;

        public FirmaService(ApplicationDbContext context, ILogger<FirmaService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ServiceResult<Firma>> CreateFirmaAsync(Firma firma, Guid userId)
        {
            try
            {
                firma.Id = Guid.NewGuid();
                firma.CreatedDate = DateTime.UtcNow;
                firma.CreatedByUserId = userId;
                firma.Durum = FirmaDurumu.Beklemede;

                _context.Firmalar.Add(firma);
                await _context.SaveChangesAsync();

                return ServiceResult<Firma>.SuccessResult(firma, "Firma başarıyla oluşturuldu.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Firma oluşturulurken hata");
                return ServiceResult<Firma>.ErrorResult("Firma oluşturulurken hata oluştu: " + ex.Message);
            }
        }

        public async Task<ServiceResult<Firma>> UpdateFirmaAsync(Firma firma, Guid userId)
        {
            try
            {
                var existing = await _context.Firmalar.FindAsync(firma.Id);
                if (existing == null || existing.IsDeleted)
                {
                    return ServiceResult<Firma>.ErrorResult("Firma bulunamadı.");
                }

                existing.Ad = firma.Ad;
                existing.VergiNo = firma.VergiNo;
                existing.Adres = firma.Adres;
                existing.Telefon = firma.Telefon;
                existing.Email = firma.Email;
                existing.WebSitesi = firma.WebSitesi;
                existing.FirmaTipi = firma.FirmaTipi;
                existing.ModifiedDate = DateTime.UtcNow;
                existing.ModifiedByUserId = userId;

                await _context.SaveChangesAsync();

                return ServiceResult<Firma>.SuccessResult(existing, "Firma başarıyla güncellendi.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Firma güncellenirken hata");
                return ServiceResult<Firma>.ErrorResult("Firma güncellenirken hata oluştu: " + ex.Message);
            }
        }

        public async Task<ServiceResult<Firma>> GetFirmaByIdAsync(Guid id)
        {
            try
            {
                var firma = await _context.Firmalar
                    .Include(f => f.Kullanicilar)
                    .Include(f => f.Malzemeler)
                    .FirstOrDefaultAsync(f => f.Id == id && !f.IsDeleted);

                if (firma == null)
                {
                    return ServiceResult<Firma>.ErrorResult("Firma bulunamadı.");
                }

                return ServiceResult<Firma>.SuccessResult(firma);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Firma getirilirken hata");
                return ServiceResult<Firma>.ErrorResult("Firma getirilirken hata oluştu: " + ex.Message);
            }
        }

        public async Task<ServiceResult<List<Firma>>> GetAllFirmalarAsync()
        {
            try
            {
                var firmalar = await _context.Firmalar
                    .Where(f => !f.IsDeleted)
                    .OrderBy(f => f.Ad)
                    .ToListAsync();

                return ServiceResult<List<Firma>>.SuccessResult(firmalar);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Firmalar listelenirken hata");
                return ServiceResult<List<Firma>>.ErrorResult("Firmalar listelenirken hata oluştu: " + ex.Message);
            }
        }

        public async Task<ServiceResult<List<Firma>>> GetFirmalarByTipAsync(FirmaTipi tip)
        {
            try
            {
                var firmalar = await _context.Firmalar
                    .Where(f => !f.IsDeleted && f.FirmaTipi == tip)
                    .OrderBy(f => f.Ad)
                    .ToListAsync();

                return ServiceResult<List<Firma>>.SuccessResult(firmalar);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Firmalar listelenirken hata");
                return ServiceResult<List<Firma>>.ErrorResult("Firmalar listelenirken hata oluştu: " + ex.Message);
            }
        }

        public async Task<ServiceResult> DeleteFirmaAsync(Guid id, Guid userId)
        {
            try
            {
                var firma = await _context.Firmalar.FindAsync(id);
                if (firma == null)
                {
                    return ServiceResult.ErrorResult("Firma bulunamadı.");
                }

                firma.IsDeleted = true;
                firma.ModifiedDate = DateTime.UtcNow;
                firma.ModifiedByUserId = userId;

                await _context.SaveChangesAsync();

                return ServiceResult.SuccessResult("Firma başarıyla silindi.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Firma silinirken hata");
                return ServiceResult.ErrorResult("Firma silinirken hata oluştu: " + ex.Message);
            }
        }

        public async Task<ServiceResult> OnaylaFirmaAsync(Guid id, Guid userId)
        {
            try
            {
                var firma = await _context.Firmalar.FindAsync(id);
                if (firma == null || firma.IsDeleted)
                {
                    return ServiceResult.ErrorResult("Firma bulunamadı.");
                }

                firma.Durum = FirmaDurumu.Onaylandi;
                firma.OnayTarihi = DateTime.UtcNow;
                firma.ModifiedDate = DateTime.UtcNow;
                firma.ModifiedByUserId = userId;

                await _context.SaveChangesAsync();

                return ServiceResult.SuccessResult("Firma başarıyla onaylandı.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Firma onaylanırken hata");
                return ServiceResult.ErrorResult("Firma onaylanırken hata oluştu: " + ex.Message);
            }
        }

        public async Task<ServiceResult> ReddetFirmaAsync(Guid id, string redNedeni, Guid userId)
        {
            try
            {
                var firma = await _context.Firmalar.FindAsync(id);
                if (firma == null || firma.IsDeleted)
                {
                    return ServiceResult.ErrorResult("Firma bulunamadı.");
                }

                firma.Durum = FirmaDurumu.Reddedildi;
                firma.ModifiedDate = DateTime.UtcNow;
                firma.ModifiedByUserId = userId;

                await _context.SaveChangesAsync();

                return ServiceResult.SuccessResult("Firma başarıyla reddedildi.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Firma reddedilirken hata");
                return ServiceResult.ErrorResult("Firma reddedilirken hata oluştu: " + ex.Message);
            }
        }

        public async Task<ServiceResult<FirmaIliskisi>> CreateFirmaIliskisiAsync(Guid musteriId, Guid tedarikciId, Guid userId)
        {
            try
            {
                var iliskiVarMi = await _context.FirmaIliskileri
                    .AnyAsync(fi => fi.MusteriFirmaId == musteriId && 
                                   fi.TedarikciFirmaId == tedarikciId && 
                                   !fi.IsDeleted);

                if (iliskiVarMi)
                {
                    return ServiceResult<FirmaIliskisi>.ErrorResult("Bu ilişki zaten mevcut.");
                }

                var iliski = new FirmaIliskisi
                {
                    Id = Guid.NewGuid(),
                    MusteriFirmaId = musteriId,
                    TedarikciFirmaId = tedarikciId,
                    Durum = FirmaDurumu.Onaylandi,
                    CreatedDate = DateTime.UtcNow,
                    CreatedByUserId = userId
                };

                _context.FirmaIliskileri.Add(iliski);
                await _context.SaveChangesAsync();

                return ServiceResult<FirmaIliskisi>.SuccessResult(iliski, "İlişki başarıyla oluşturuldu.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Firma ilişkisi oluşturulurken hata");
                return ServiceResult<FirmaIliskisi>.ErrorResult("İlişki oluşturulurken hata: " + ex.Message);
            }
        }

        public async Task<ServiceResult<List<Firma>>> GetMusterininTedarikcileriAsync(Guid musteriId)
        {
            try
            {
                var tedarikçiler = await _context.FirmaIliskileri
                    .Where(fi => fi.MusteriFirmaId == musteriId && !fi.IsDeleted)
                    .Include(fi => fi.TedarikciFirma)
                    .Select(fi => fi.TedarikciFirma)
                    .ToListAsync();

                return ServiceResult<List<Firma>>.SuccessResult(tedarikçiler);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Tedarikçiler getirilirken hata");
                return ServiceResult<List<Firma>>.ErrorResult("Tedarikçiler getirilirken hata: " + ex.Message);
            }
        }

        public async Task<ServiceResult<List<Firma>>> GetTedarikcininMusterileriAsync(Guid tedarikciId)
        {
            try
            {
                var musteriler = await _context.FirmaIliskileri
                    .Where(fi => fi.TedarikciFirmaId == tedarikciId && !fi.IsDeleted)
                    .Include(fi => fi.MusteriFirma)
                    .Select(fi => fi.MusteriFirma)
                    .ToListAsync();

                return ServiceResult<List<Firma>>.SuccessResult(musteriler);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Müşteriler getirilirken hata");
                return ServiceResult<List<Firma>>.ErrorResult("Müşteriler getirilirken hata: " + ex.Message);
            }
        }
    }
}
