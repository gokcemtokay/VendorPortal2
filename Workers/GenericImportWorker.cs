using System.Text.Json;
using VendorPortal.Models.DTOs;
using VendorPortal.Services;

namespace VendorPortal.Workers
{
    /// <summary>
    /// Arka plan import işlemi worker
    /// </summary>
    public class GenericImportWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<GenericImportWorker> _logger;

        public GenericImportWorker(
            IServiceProvider serviceProvider,
            ILogger<GenericImportWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("GenericImportWorker başlatıldı.");

            while (!stoppingToken.IsCancellationRequested)
            {
                // Worker'ın düzenli çalışması için 10 saniye bekle
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }

            _logger.LogInformation("GenericImportWorker durduruldu.");
        }

        /// <summary>
        /// Sipariş import işlemini başlat
        /// </summary>
        public async Task<bool> ProcessSiparisImportAsync(string jsonData, Guid userId)
        {
            try
            {
                _logger.LogInformation("Sipariş import işlemi başlatıldı.");

                // JSON'u parse et
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var siparislerDto = JsonSerializer.Deserialize<SiparislerDto>(jsonData, options);

                if (siparislerDto?.Siparisler == null || !siparislerDto.Siparisler.Any())
                {
                    _logger.LogWarning("JSON içinde sipariş bulunamadı.");
                    return false;
                }

                // Scope oluştur ve servisi al
                using var scope = _serviceProvider.CreateScope();
                var siparisService = scope.ServiceProvider.GetRequiredService<ISiparisService>();

                // Siparişleri toplu olarak oluştur
                var result = await siparisService.BulkCreateSiparislerAsync(siparislerDto.Siparisler, userId);

                if (result.Success)
                {
                    _logger.LogInformation($"{result.Data?.Count ?? 0} sipariş başarıyla import edildi.");
                    return true;
                }
                else
                {
                    _logger.LogError($"Sipariş import hatası: {result.Message}");
                    foreach (var error in result.Errors)
                    {
                        _logger.LogError($"  - {error}");
                    }
                    return false;
                }
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError(jsonEx, "JSON parse hatası");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sipariş import işlemi sırasında beklenmeyen hata");
                return false;
            }
        }
    }
}
