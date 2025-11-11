using VendorPortal.Models;
using VendorPortal.Models.Entities;

namespace VendorPortal.Services
{
    /// <summary>
    /// İhale servisi interface
    /// </summary>
    public interface IIhaleService
    {
        Task<ServiceResult<Ihale>> CreateIhaleAsync(Ihale ihale, List<IhaleKalem> kalemler, Guid userId);
        Task<ServiceResult<Ihale>> UpdateIhaleAsync(Ihale ihale, Guid userId);
        Task<ServiceResult<Ihale>> GetIhaleByIdAsync(Guid id);
        Task<ServiceResult<List<Ihale>>> GetIhalelerByFirmaAsync(Guid firmaId);
        Task<ServiceResult<List<Ihale>>> GetAllIhalelerAsync();
        Task<ServiceResult> DeleteIhaleAsync(Guid id, Guid userId);
        Task<ServiceResult> YayinlaIhaleAsync(Guid id, Guid userId);
        Task<ServiceResult> IptalEtIhaleAsync(Guid id, string iptalNedeni, Guid userId);
        Task<ServiceResult<IhaleDaveti>> CreateDavetAsync(Guid ihaleId, Guid tedarikciFirmaId, Guid userId);
        Task<ServiceResult<IhaleTeklif>> CreateTeklifAsync(IhaleTeklif teklif, List<IhaleTeklifKalem> kalemler, Guid userId);
        Task<ServiceResult<List<IhaleTeklif>>> GetTekliflerByIhaleAsync(Guid ihaleId);
        Task<ServiceResult> OnaylaTeklifAsync(Guid teklifId, Guid userId);
    }
}
