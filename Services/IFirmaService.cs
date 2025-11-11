using VendorPortal.Models;
using VendorPortal.Models.Entities;
using VendorPortal.Models.Enums;

namespace VendorPortal.Services
{
    /// <summary>
    /// Firma servisi interface
    /// </summary>
    public interface IFirmaService
    {
        Task<ServiceResult<Firma>> CreateFirmaAsync(Firma firma, Guid userId);
        Task<ServiceResult<Firma>> UpdateFirmaAsync(Firma firma, Guid userId);
        Task<ServiceResult<Firma>> GetFirmaByIdAsync(Guid id);
        Task<ServiceResult<List<Firma>>> GetAllFirmalarAsync();
        Task<ServiceResult<List<Firma>>> GetFirmalarByTipAsync(FirmaTipi tip);
        Task<ServiceResult> DeleteFirmaAsync(Guid id, Guid userId);
        Task<ServiceResult> OnaylaFirmaAsync(Guid id, Guid userId);
        Task<ServiceResult> ReddetFirmaAsync(Guid id, string redNedeni, Guid userId);
        Task<ServiceResult<FirmaIliskisi>> CreateFirmaIliskisiAsync(Guid musteriId, Guid tedarikciId, Guid userId);
        Task<ServiceResult<List<Firma>>> GetMusterininTedarikcileriAsync(Guid musteriId);
        Task<ServiceResult<List<Firma>>> GetTedarikcininMusterileriAsync(Guid tedarikciId);
    }
}
