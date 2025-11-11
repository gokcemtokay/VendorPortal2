using VendorPortal.Models;
using VendorPortal.Models.DTOs;
using VendorPortal.Models.Entities;

namespace VendorPortal.Services
{
    /// <summary>
    /// Sipariş servisi interface
    /// </summary>
    public interface ISiparisService
    {
        Task<ServiceResult<SiparisBaslik>> CreateSiparisAsync(SiparisBaslikDto dto, Guid userId);
        Task<ServiceResult<List<SiparisBaslik>>> BulkCreateSiparislerAsync(List<SiparisBaslikDto> dtoList, Guid userId);
        Task<ServiceResult<SiparisBaslik>> GetSiparisByIdAsync(Guid id);
        Task<ServiceResult<List<SiparisBaslik>>> GetSiparislerByFirmaAsync(Guid firmaId, bool isMusteriView);
        Task<ServiceResult> UpdateSiparisDurumAsync(Guid siparisId, int yeniDurum, Guid userId);
    }
}
