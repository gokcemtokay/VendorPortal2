using Microsoft.EntityFrameworkCore;
using VendorPortal.Data;
using VendorPortal.Models;
using VendorPortal.Models.Entities;

namespace VendorPortal.Services
{
    /// <summary>
    /// Malzeme servisi implementasyonu
    /// </summary>
    public class MalzemeService : IMalzemeService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MalzemeService> _logger;

        public MalzemeService(ApplicationDbContext context, ILogger<MalzemeService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ServiceResult<Malzeme>> CreateMalzemeAsync(Malzeme malzeme, Guid userId)
        {
            try
            {
                // Aynı firma içinde aynı kodda malzeme var mı kontrol et
                var mevcutMalzeme = await _context.Malzemeler
                    .FirstOrDefaultAsync(m => m.FirmaId == malzeme.FirmaId && 
                                             m.Kod == malzeme.Kod && 
                                             !m.IsDeleted);

                if (mevcutMalzeme != null)
                {
                    return ServiceResult<Malzeme>.ErrorResult("Bu malzeme kodu zaten kullanılıyor.");
                }

                malzeme.Id = Guid.NewGuid();
                malzeme.CreatedDate = DateTime.UtcNow;
                malzeme.CreatedByUserId = userId;

                _context.Malzemeler.Add(malzeme);
                await _context.SaveChangesAsync();

                return ServiceResult<Malzeme>.SuccessResult(malzeme, "Malzeme başarıyla oluşturuldu.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Malzeme oluşturulurken hata");
                return ServiceResult<Malzeme>.ErrorResult("Malzeme oluşturulurken hata oluştu: " + ex.Message);
            }
        }

        public async Task<ServiceResult<Malzeme>> UpdateMalzemeAsync(Malzeme malzeme, Guid userId)
        {
            try
            {
                var existing = await _context.Malzemeler.FindAsync(malzeme.Id);
                if (existing == null || existing.IsDeleted)
                {
                    return ServiceResult<Malzeme>.ErrorResult("Malzeme bulunamadı.");
                }

                existing.Kod = malzeme.Kod;
                existing.Ad = malzeme.Ad;
                existing.Aciklama = malzeme.Aciklama;
                existing.Birim = malzeme.Birim;
                existing.Fiyat = malzeme.Fiyat;
                existing.ParaBirimi = malzeme.ParaBirimi;

                await _context.SaveChangesAsync();

                return ServiceResult<Malzeme>.SuccessResult(existing, "Malzeme başarıyla güncellendi.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Malzeme güncellenirken hata");
                return ServiceResult<Malzeme>.ErrorResult("Malzeme güncellenirken hata oluştu: " + ex.Message);
            }
        }

        public async Task<ServiceResult<Malzeme>> GetMalzemeByIdAsync(Guid id)
        {
            try
            {
                var malzeme = await _context.Malzemeler
                    .Include(m => m.Firma)
                    .FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted);

                if (malzeme == null)
                {
                    return ServiceResult<Malzeme>.ErrorResult("Malzeme bulunamadı.");
                }

                return ServiceResult<Malzeme>.SuccessResult(malzeme);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Malzeme getirilirken hata");
                return ServiceResult<Malzeme>.ErrorResult("Malzeme getirilirken hata oluştu: " + ex.Message);
            }
        }

        public async Task<ServiceResult<List<Malzeme>>> GetMalzemelerByFirmaAsync(Guid firmaId)
        {
            try
            {
                var malzemeler = await _context.Malzemeler
                    .Where(m => m.FirmaId == firmaId && !m.IsDeleted)
                    .Include(m => m.Firma)
                    .OrderBy(m => m.Kod)
                    .ToListAsync();

                return ServiceResult<List<Malzeme>>.SuccessResult(malzemeler);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Malzemeler listelenirken hata");
                return ServiceResult<List<Malzeme>>.ErrorResult("Malzemeler listelenirken hata oluştu: " + ex.Message);
            }
        }

        public async Task<ServiceResult<List<Malzeme>>> GetAllMalzemelerAsync()
        {
            try
            {
                var malzemeler = await _context.Malzemeler
                    .Where(m => !m.IsDeleted)
                    .Include(m => m.Firma)
                    .OrderBy(m => m.Kod)
                    .ToListAsync();

                return ServiceResult<List<Malzeme>>.SuccessResult(malzemeler);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Malzemeler listelenirken hata");
                return ServiceResult<List<Malzeme>>.ErrorResult("Malzemeler listelenirken hata oluştu: " + ex.Message);
            }
        }

        public async Task<ServiceResult> DeleteMalzemeAsync(Guid id, Guid userId)
        {
            try
            {
                var malzeme = await _context.Malzemeler.FindAsync(id);
                if (malzeme == null)
                {
                    return ServiceResult.ErrorResult("Malzeme bulunamadı.");
                }

                malzeme.IsDeleted = true;

                await _context.SaveChangesAsync();

                return ServiceResult.SuccessResult("Malzeme başarıyla silindi.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Malzeme silinirken hata");
                return ServiceResult.ErrorResult("Malzeme silinirken hata oluştu: " + ex.Message);
            }
        }

        public async Task<ServiceResult<TedarikciMalzemeEslestirme>> CreateEslestirmeAsync(
            Guid musteriMalzemeId, Guid tedarikciMalzemeId, Guid userId)
        {
            try
            {
                var musteriMalzeme = await _context.Malzemeler.FindAsync(musteriMalzemeId);
                var tedarikciMalzeme = await _context.Malzemeler.FindAsync(tedarikciMalzemeId);

                if (musteriMalzeme == null || tedarikciMalzeme == null)
                {
                    return ServiceResult<TedarikciMalzemeEslestirme>.ErrorResult("Malzemeler bulunamadı.");
                }

                // Mevcut eşleştirme var mı kontrol et
                var mevcutEslestirme = await _context.TedarikciMalzemeEslestirmeleri
                    .FirstOrDefaultAsync(e => 
                        e.MusteriMalzemeId == musteriMalzemeId && 
                        e.TedarikciMalzemeId == tedarikciMalzemeId && 
                        !e.IsDeleted);

                if (mevcutEslestirme != null)
                {
                    return ServiceResult<TedarikciMalzemeEslestirme>.ErrorResult("Bu eşleştirme zaten mevcut.");
                }

                var eslestirme = new TedarikciMalzemeEslestirme
                {
                    Id = Guid.NewGuid(),
                    MusteriFirmaId = musteriMalzeme.FirmaId,
                    MusteriMalzemeId = musteriMalzemeId,
                    TedarikciFirmaId = tedarikciMalzeme.FirmaId,
                    TedarikciMalzemeId = tedarikciMalzemeId,
                    CreatedDate = DateTime.UtcNow,
                    CreatedByUserId = userId
                };

                _context.TedarikciMalzemeEslestirmeleri.Add(eslestirme);
                await _context.SaveChangesAsync();

                return ServiceResult<TedarikciMalzemeEslestirme>.SuccessResult(
                    eslestirme, "Eşleştirme başarıyla oluşturuldu.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Eşleştirme oluşturulurken hata");
                return ServiceResult<TedarikciMalzemeEslestirme>.ErrorResult(
                    "Eşleştirme oluşturulurken hata oluştu: " + ex.Message);
            }
        }

        public async Task<ServiceResult<List<TedarikciMalzemeEslestirme>>> GetEslestirmelerAsync(
            Guid musteriId, Guid tedarikciId)
        {
            try
            {
                var eslestirmeler = await _context.TedarikciMalzemeEslestirmeleri
                    .Where(e => e.MusteriFirmaId == musteriId && 
                               e.TedarikciFirmaId == tedarikciId && 
                               !e.IsDeleted)
                    .Include(e => e.MusteriMalzeme)
                    .Include(e => e.TedarikciMalzeme)
                    .ToListAsync();

                return ServiceResult<List<TedarikciMalzemeEslestirme>>.SuccessResult(eslestirmeler);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Eşleştirmeler getirilirken hata");
                return ServiceResult<List<TedarikciMalzemeEslestirme>>.ErrorResult(
                    "Eşleştirmeler getirilirken hata oluştu: " + ex.Message);
            }
        }
    }
}
