using VendorPortal.Models;
using VendorPortal.Models.Entities;

namespace VendorPortal.Services
{
    /// <summary>
    /// Malzeme servisi interface
    /// </summary>
    public interface IMalzemeService
    {
        Task<ServiceResult<Malzeme>> CreateMalzemeAsync(Malzeme malzeme, Guid userId);
        Task<ServiceResult<Malzeme>> UpdateMalzemeAsync(Malzeme malzeme, Guid userId);
        Task<ServiceResult<Malzeme>> GetMalzemeByIdAsync(Guid id);
        Task<ServiceResult<List<Malzeme>>> GetMalzemelerByFirmaAsync(Guid firmaId);
        Task<ServiceResult<List<Malzeme>>> GetAllMalzemelerAsync();
        Task<ServiceResult> DeleteMalzemeAsync(Guid id, Guid userId);
        Task<ServiceResult<TedarikciMalzemeEslestirme>> CreateEslestirmeAsync(Guid musteriMalzemeId, Guid tedarikciMalzemeId, Guid userId);
        Task<ServiceResult<List<TedarikciMalzemeEslestirme>>> GetEslestirmelerAsync(Guid musteriId, Guid tedarikciId);
    }
}
