namespace VendorPortal.Models.Enums
{
    /// <summary>
    /// Firma tipi enum'u
    /// </summary>
    public enum FirmaTipi
    {
        Musteri = 1,
        Tedarikci = 2,
        HerIkisi = 3
    }

    /// <summary>
    /// Firma durumu enum'u
    /// </summary>
    public enum FirmaDurumu
    {
        Beklemede = 1,
        Onaylandi = 2,
        Reddedildi = 3,
        Pasif = 4
    }

    /// <summary>
    /// Sipariş türü enum'u
    /// </summary>
    public enum SiparisTuru
    {
        SatinAlma = 1,
        Satis = 2
    }

    /// <summary>
    /// Sipariş durumu enum'u
    /// </summary>
    public enum SiparisDurumu
    {
        Taslak = 0,
        Olusturuldu = 1,
        OnayaGitti = 2,
        TedarikciOnayladi = 3,
        RevizeEdildi = 4,
        Onaylandi = 5,
        Reddedildi = 6,
        Kapatildi = 7
    }

    /// <summary>
    /// Sipariş kalemi durumu enum'u
    /// </summary>
    public enum SiparisKalemDurumu
    {
        Bekliyor = 0,
        MusteriRevizeEtti = 1,
        TedarikciRevizeEtti = 2,
        Onaylandi = 3,
        Reddedildi = 4
    }

    /// <summary>
    /// İhale türü enum'u
    /// </summary>
    public enum IhaleTuru
    {
        HerkeseAcik = 1,
        Davetli = 2,
        Kapali = 3
    }

    /// <summary>
    /// İhale durumu enum'u
    /// </summary>
    public enum IhaleDurumu
    {
        Taslak = 0,
        Yayinda = 1,
        Degerlendiriliyor = 2,
        Tamamlandi = 3,
        IptalEdildi = 4
    }

    /// <summary>
    /// Teklif durumu enum'u
    /// </summary>
    public enum TeklifDurumu
    {
        Taslak = 0,
        Gonderildi = 1,
        Reddedildi = 2,
        Onaylandi = 3
    }

    /// <summary>
    /// Teklif kalemi durumu enum'u
    /// </summary>
    public enum TeklifKalemDurumu
    {
        Bekliyor = 0,
        MusteriOnayladi = 1,
        MusteriReddetti = 2
    }

    /// <summary>
    /// İşlem tipi enum'u (Sipariş geçmişi için)
    /// </summary>
    public enum IslemTipi
    {
        Olusturuldu = 1,
        Guncellendi = 2,
        RevizeEdildi = 3,
        Onaylandi = 4,
        Reddedildi = 5
    }

    /// <summary>
    /// Doküman kategorisi enum'u
    /// </summary>
    public enum DokumanKategorisi
    {
        Kalite = 1,
        SatinAlma = 2,
        Sozlesme = 3,
        Sertifika = 4,
        Diger = 5
    }

    /// <summary>
    /// Mail bildirimi durumu enum'u
    /// </summary>
    public enum MailDurumu
    {
        Beklemede = 0,
        Gonderildi = 1,
        Gonderilemedi = 2
    }

    /// <summary>
    /// Mail referans tipi enum'u
    /// </summary>
    public enum MailReferansTipi
    {
        Siparis = 1,
        Ihale = 2,
        FirmaBasvuru = 3,
        Teklif = 4
    }
}
