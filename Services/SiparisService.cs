using Microsoft.EntityFrameworkCore;
using VendorPortal.Data;
using VendorPortal.Models;
using VendorPortal.Models.DTOs;
using VendorPortal.Models.Entities;
using VendorPortal.Models.Enums;

namespace VendorPortal.Services
{
    /// <summary>
    /// Sipariş servisi implementasyonu
    /// </summary>
    public class SiparisService : ISiparisService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SiparisService> _logger;

        public SiparisService(ApplicationDbContext context, ILogger<SiparisService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ServiceResult<SiparisBaslik>> CreateSiparisAsync(SiparisBaslikDto dto, Guid userId)
        {
            try
            {
                // Müşteri ve Tedarikçi firmalarını bul
                var musteriFirma = await _context.Firmalar
                    .FirstOrDefaultAsync(f => f.VergiNo == dto.MusteriVergiNo);

                var tedarikciFirma = await _context.Firmalar
                    .FirstOrDefaultAsync(f => f.VergiNo == dto.TedarikciVergiNo);

                if (musteriFirma == null || tedarikciFirma == null)
                {
                    return ServiceResult<SiparisBaslik>.ErrorResult("Müşteri veya Tedarikçi firma bulunamadı.");
                }

                // Sipariş oluştur
                var siparis = new SiparisBaslik
                {
                    Id = Guid.NewGuid(),
                    ReferansNo = dto.SiparisNo,
                    SiparisTarihi = dto.SiparisTarihi,
                    SiparisTuru = (SiparisTuru)dto.SiparisTipi,
                    MusteriFirmaId = musteriFirma.Id,
                    TedarikciFirmaId = tedarikciFirma.Id,
                    ParaBirimi = dto.ParaBirimi,
                    TeslimatAdresi = dto.TeslimatAdresi,
                    Durum = SiparisDurumu.Olusturuldu,
                    CreatedDate = dto.KayitTarihi,
                    CreatedByUserId = userId
                };

                // Kalemleri ekle
                foreach (var kalemDto in dto.Kalemler)
                {
                    // Malzemeyi bul (Tedarikçinin malzemesinden)
                    var malzeme = await _context.Malzemeler
                        .FirstOrDefaultAsync(m => m.Kod == kalemDto.MalzemeNum && m.FirmaId == tedarikciFirma.Id);

                    if (malzeme == null)
                    {
                        _logger.LogWarning($"Malzeme bulunamadı: {kalemDto.MalzemeNum}");
                        continue;
                    }

                    var kalem = new SiparisKalem
                    {
                        Id = Guid.NewGuid(),
                        SiparisBaslikId = siparis.Id,
                        MalzemeId = malzeme.Id,
                        MalzemeKodu = malzeme.Kod,
                        MalzemeAdi = malzeme.Ad,
                        Miktar = kalemDto.Miktar,
                        Birim = malzeme.Birim ?? "Adet",
                        BirimFiyat = kalemDto.Fiyat,
                        ToplamFiyat = kalemDto.Miktar * kalemDto.Fiyat - kalemDto.IndirimTutari,
                        IstenenTeslimatTarihi = kalemDto.IstenenTeslimTarihi,
                        Aciklama = kalemDto.MusteriNotu,
                        Durum = SiparisKalemDurumu.Bekliyor,
                        CreatedDate = kalemDto.KayitTarihi,
                        CreatedByUserId = userId
                    };

                    siparis.Kalemler.Add(kalem);
                }

                // Toplam tutarı hesapla
                siparis.ToplamTutar = siparis.Kalemler.Sum(k => k.ToplamFiyat);

                _context.SiparisBasliklar.Add(siparis);
                await _context.SaveChangesAsync();

                // Geçmişe kaydet
                var gecmis = new SiparisOnayGecmisi
                {
                    Id = Guid.NewGuid(),
                    SiparisBaslikId = siparis.Id,
                    IslemTipi = IslemTipi.Olusturuldu,
                    Aciklama = "Sipariş oluşturuldu",
                    YeniDurum = (int)siparis.Durum,
                    IslemYapanUserId = userId,
                    IslemTarihi = DateTime.UtcNow
                };

                _context.SiparisOnayGecmisleri.Add(gecmis);
                await _context.SaveChangesAsync();

                return ServiceResult<SiparisBaslik>.SuccessResult(siparis, "Sipariş başarıyla oluşturuldu.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sipariş oluşturulurken hata oluştu");
                return ServiceResult<SiparisBaslik>.ErrorResult("Sipariş oluşturulurken hata oluştu: " + ex.Message);
            }
        }

        public async Task<ServiceResult<List<SiparisBaslik>>> BulkCreateSiparislerAsync(List<SiparisBaslikDto> dtoList, Guid userId)
        {
            var siparisler = new List<SiparisBaslik>();
            var errors = new List<string>();

            foreach (var dto in dtoList)
            {
                var result = await CreateSiparisAsync(dto, userId);
                if (result.Success && result.Data != null)
                {
                    siparisler.Add(result.Data);
                }
                else
                {
                    errors.Add($"Sipariş {dto.SiparisNo}: {result.Message}");
                }
            }

            if (siparisler.Any())
            {
                return ServiceResult<List<SiparisBaslik>>.SuccessResult(
                    siparisler,
                    $"{siparisler.Count} sipariş başarıyla oluşturuldu."
                );
            }

            return ServiceResult<List<SiparisBaslik>>.ErrorResult(
                "Hiçbir sipariş oluşturulamadı.",
                errors
            );
        }

        public async Task<ServiceResult<SiparisBaslik>> GetSiparisByIdAsync(Guid id)
        {
            try
            {
                var siparis = await _context.SiparisBasliklar
                    .Include(s => s.MusteriFirma)
                    .Include(s => s.TedarikciFirma)
                    .Include(s => s.Kalemler)
                        .ThenInclude(k => k.Malzeme)
                    .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);

                if (siparis == null)
                {
                    return ServiceResult<SiparisBaslik>.ErrorResult("Sipariş bulunamadı.");
                }

                return ServiceResult<SiparisBaslik>.SuccessResult(siparis);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sipariş getirilirken hata oluştu");
                return ServiceResult<SiparisBaslik>.ErrorResult("Sipariş getirilirken hata oluştu: " + ex.Message);
            }
        }

        public async Task<ServiceResult<List<SiparisBaslik>>> GetSiparislerByFirmaAsync(Guid firmaId, bool isMusteriView)
        {
            try
            {
                IQueryable<SiparisBaslik> query = _context.SiparisBasliklar
                    .Include(s => s.MusteriFirma)
                    .Include(s => s.TedarikciFirma)
                    .Include(s => s.Kalemler)
                    .Where(s => !s.IsDeleted);

                if (isMusteriView)
                {
                    query = query.Where(s => s.MusteriFirmaId == firmaId);
                }
                else
                {
                    query = query.Where(s => s.TedarikciFirmaId == firmaId);
                }

                var siparisler = await query
                    .OrderByDescending(s => s.SiparisTarihi)
                    .ToListAsync();

                return ServiceResult<List<SiparisBaslik>>.SuccessResult(siparisler);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Siparişler getirilirken hata oluştu");
                return ServiceResult<List<SiparisBaslik>>.ErrorResult("Siparişler getirilirken hata oluştu: " + ex.Message);
            }
        }

        public async Task<ServiceResult> UpdateSiparisDurumAsync(Guid siparisId, int yeniDurum, Guid userId)
        {
            try
            {
                var siparis = await _context.SiparisBasliklar.FindAsync(siparisId);
                if (siparis == null)
                {
                    return ServiceResult.ErrorResult("Sipariş bulunamadı.");
                }

                var eskiDurum = (int)siparis.Durum;
                siparis.Durum = (SiparisDurumu)yeniDurum;
                siparis.ModifiedDate = DateTime.UtcNow;
                siparis.ModifiedByUserId = userId;

                // Geçmişe kaydet
                var gecmis = new SiparisOnayGecmisi
                {
                    Id = Guid.NewGuid(),
                    SiparisBaslikId = siparisId,
                    IslemTipi = IslemTipi.Guncellendi,
                    Aciklama = $"Sipariş durumu güncellendi: {(SiparisDurumu)eskiDurum} -> {(SiparisDurumu)yeniDurum}",
                    EskiDurum = eskiDurum,
                    YeniDurum = yeniDurum,
                    IslemYapanUserId = userId,
                    IslemTarihi = DateTime.UtcNow
                };

                _context.SiparisOnayGecmisleri.Add(gecmis);
                await _context.SaveChangesAsync();

                return ServiceResult.SuccessResult("Sipariş durumu güncellendi.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sipariş durumu güncellenirken hata oluştu");
                return ServiceResult.ErrorResult("Sipariş durumu güncellenirken hata oluştu: " + ex.Message);
            }
        }
    }
}
