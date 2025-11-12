using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VendorPortal.Models.Entities;
using System; // DateTime için gerekli

namespace VendorPortal.Data
{
    /// <summary>
    /// Ana veritabanı context sınıfı
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSet'ler (null! ataması, C# 8.0+ Nullable Reference Types için iyi bir pratik)
        public DbSet<Firma> Firmalar { get; set; } = null!;
        public DbSet<FirmaIliskisi> FirmaIliskileri { get; set; } = null!;
        public DbSet<Malzeme> Malzemeler { get; set; } = null!;
        public DbSet<TedarikciMalzemeEslestirme> TedarikciMalzemeEslestirmeleri { get; set; } = null!;
        public DbSet<SiparisBaslik> SiparisBasliklar { get; set; } = null!;
        public DbSet<SiparisKalem> SiparisKalemler { get; set; } = null!;
        public DbSet<SiparisOnayGecmisi> SiparisOnayGecmisleri { get; set; } = null!;
        public DbSet<Ihale> Ihaleler { get; set; } = null!;
        public DbSet<IhaleKalem> IhaleKalemleri { get; set; } = null!;
        public DbSet<IhaleDaveti> IhaleDavetleri { get; set; } = null!;
        public DbSet<IhaleTeklif> IhaleTeklifleri { get; set; } = null!;
        public DbSet<IhaleTeklifKalem> IhaleTeklifKalemleri { get; set; } = null!;
        public DbSet<IhaleDokuman> IhaleDokumanlari { get; set; } = null!;
        public DbSet<Dokuman> Dokumanlar { get; set; } = null!;
        public DbSet<MailBildirimi> MailBildirimleri { get; set; } = null!;
        public DbSet<UserFirmaYetkisi> UserFirmaYetkileri { get; set; } = null!;
        public DbSet<Bildirim> Bildirimler { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // --- FİRMA ile ApplicationUser arasındaki TÜM ilişkileri tanımlıyoruz ---

            // 1. ApplicationUser'ın ait olduğu Firma (ApplicationUser.Firma - Firma.Kullanicilar)
            builder.Entity<ApplicationUser>()
                .HasOne(u => u.Firma)                // Kullanıcının ait olduğu Firma
                .WithMany(f => f.Kullanicilar)       // Firmanın sahip olduğu kullanıcılar koleksiyonu
                .HasForeignKey(u => u.FirmaId)       // ApplicationUser'daki FK
                .IsRequired(false);                  // FK null olabilir

            // 2. Firma'yı oluşturan kullanıcı (Firma.CreatedByUser)
            builder.Entity<Firma>()
                .HasOne(f => f.CreatedByUser)        // Firma'yı oluşturan kullanıcı
                .WithMany()                          // ApplicationUser'da bu ilişki için bir koleksiyon yok (tekil ilişkilerde WithMany() veya WithMany(null) kullanılabilir)
                .HasForeignKey(f => f.CreatedByUserId) // Firma'daki FK
                .IsRequired(false)                   // FK null olabilir
                .OnDelete(DeleteBehavior.Restrict);  // Silme davranışını belirt

            // 3. Firma'yı en son değiştiren kullanıcı (Firma.ModifiedByUser)
            builder.Entity<Firma>()
                .HasOne(f => f.ModifiedByUser)       // Firma'yı en son değiştiren kullanıcı
                .WithMany()                          // ApplicationUser'da bu ilişki için bir koleksiyon yok
                .HasForeignKey(f => f.ModifiedByUserId) // Firma'daki FK
                .IsRequired(false)                   // FK null olabilir
                .OnDelete(DeleteBehavior.Restrict);  // Silme davranışını belirt


            // --- SiparisKalem ile Malzeme arasındaki ilişkiyi açıkça tanımlayalım ---
            // Malzeme sınıfında ICollection<SiparisKalem> SiparisKalemleri koleksiyonu olduğu için WithMany() içinde bunu belirtiyoruz.
            builder.Entity<SiparisKalem>()
                .HasOne(sk => sk.Malzeme)          // SiparisKalem'in bir Malzeme'si vardır
                .WithMany(m => m.SiparisKalemleri) // Malzeme entity'sindeki SiparisKalemleri koleksiyonu
                .HasForeignKey(sk => sk.MalzemeId) // SiparisKalem'deki MalzemeId foreign key'dir
                .IsRequired(true)                  // SiparisKalem.MalzemeId non-nullable olduğu için
                .OnDelete(DeleteBehavior.Restrict);

            // SiparisKalem ve CreatedByUser arasındaki ilişki
            builder.Entity<SiparisKalem>()
                .HasOne(sk => sk.CreatedByUser)
                .WithMany() // ApplicationUser'da bu ilişki için bir koleksiyon yok
                .HasForeignKey(sk => sk.CreatedByUserId)
                .IsRequired(false) // CreatedByUserId nullable olduğu için
                .OnDelete(DeleteBehavior.Restrict); // Örneğin

            // --- Diğer mevcut ilişkileriniz ---

            // Firma ilişkileri
            builder.Entity<FirmaIliskisi>()
                .HasOne(fi => fi.MusteriFirma)
                .WithMany(f => f.MusteriOlarakIliskiler)
                .HasForeignKey(fi => fi.MusteriFirmaId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<FirmaIliskisi>()
                .HasOne(fi => fi.TedarikciFirma)
                .WithMany(f => f.TedarikciOlarakIliskiler)
                .HasForeignKey(fi => fi.TedarikciFirmaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Tedarikçi Malzeme Eşleştirme
            builder.Entity<TedarikciMalzemeEslestirme>()
                .HasOne(tme => tme.MusteriMalzeme)
                .WithMany(m => m.MusteriEslestirmeleri)
                .HasForeignKey(tme => tme.MusteriMalzemeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<TedarikciMalzemeEslestirme>()
                .HasOne(tme => tme.TedarikciMalzeme)
                .WithMany(m => m.TedarikciEslestirmeleri)
                .HasForeignKey(tme => tme.TedarikciMalzemeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Sipariş ilişkileri
            builder.Entity<SiparisBaslik>()
                .HasOne(s => s.MusteriFirma)
                .WithMany(f => f.MusteriOlarakSiparisler)
                .HasForeignKey(s => s.MusteriFirmaId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<SiparisBaslik>()
                .HasOne(s => s.TedarikciFirma)
                .WithMany(f => f.TedarikciOlarakSiparisler)
                .HasForeignKey(s => s.TedarikciFirmaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Sipariş Baslık ve CreatedByUser ilişkisi
            builder.Entity<SiparisBaslik>()
                .HasOne(sb => sb.CreatedByUser)
                .WithMany()
                .HasForeignKey(sb => sb.CreatedByUserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            // Malzeme ve Firma arasındaki ilişkiyi de ekleyelim
            // Malzeme sınıfında public Guid FirmaId { get; set; } ve public virtual Firma Firma { get; set; } = null!; olduğunu varsayıyoruz.
            builder.Entity<Malzeme>()
                .HasOne(m => m.Firma)
                .WithMany(f => f.Malzemeler)
                .HasForeignKey(m => m.FirmaId)
                .IsRequired(true) // Bir malzemenin mutlaka bir firmaya ait olduğunu varsayarsak.
                .OnDelete(DeleteBehavior.Restrict);

            // İhale ve Firma arasındaki ilişki (Bir ihale bir MüşteriFirması tarafından oluşturulur)
            // Ihale sınıfında public Guid MusteriFirmaId { get; set; } ve public virtual Firma MusteriFirma { get; set; } = null!; olduğunu varsayıyoruz.
            builder.Entity<Ihale>()
                .HasOne(i => i.MusteriFirma)
                .WithMany(f => f.Ihaleler)
                .HasForeignKey(i => i.MusteriFirmaId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);

            // ⭐ YENİ: UserFirmaYetkisi ilişkileri
            builder.Entity<UserFirmaYetkisi>()
                .HasOne(ufy => ufy.User)
                .WithMany(u => u.FirmaYetkileri)
                .HasForeignKey(ufy => ufy.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserFirmaYetkisi>()
                .HasOne(ufy => ufy.Firma)
                .WithMany()
                .HasForeignKey(ufy => ufy.FirmaId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UserFirmaYetkisi>()
                .HasOne(ufy => ufy.CreatedByUser)
                .WithMany()
                .HasForeignKey(ufy => ufy.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ⭐ YENİ: Bildirim ilişkileri
            builder.Entity<Bildirim>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bildirimler)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ⭐ YENİ: Index'ler
            builder.Entity<UserFirmaYetkisi>()
                .HasIndex(ufy => new { ufy.UserId, ufy.FirmaId });

            builder.Entity<Bildirim>()
                .HasIndex(b => new { b.UserId, b.Okundu });


            // Decimal precision ayarları
            builder.Entity<Malzeme>().Property(m => m.Fiyat).HasColumnType("numeric(18,2)");
            builder.Entity<SiparisBaslik>().Property(s => s.ToplamTutar).HasColumnType("numeric(18,2)");
            builder.Entity<SiparisKalem>().Property(sk => sk.Miktar).HasColumnType("numeric(18,2)");
            builder.Entity<SiparisKalem>().Property(sk => sk.BirimFiyat).HasColumnType("numeric(18,2)");
            builder.Entity<SiparisKalem>().Property(sk => sk.ToplamFiyat).HasColumnType("numeric(18,2)");
            builder.Entity<IhaleTeklifKalem>().Property(itk => itk.TeklifEdilenMiktar).HasColumnType("numeric(18,2)");
            builder.Entity<IhaleTeklifKalem>().Property(itk => itk.BirimFiyat).HasColumnType("numeric(18,2)");
            builder.Entity<IhaleTeklifKalem>().Property(itk => itk.ToplamFiyat).HasColumnType("numeric(18,2)");
            builder.Entity<IhaleTeklifKalem>().Property(itk => itk.IndirimOrani).HasColumnType("numeric(5,2)");

            // Index'ler
            builder.Entity<Firma>()
                .HasIndex(f => f.VergiNo)
                .IsUnique();

            builder.Entity<SiparisBaslik>()
                .HasIndex(s => s.ReferansNo)
                .IsUnique();

            builder.Entity<Ihale>()
                .HasIndex(i => i.ReferansNo)
                .IsUnique();

            builder.Entity<IhaleTeklif>()
                .HasIndex(it => it.TeklifNo)
                .IsUnique();
        }
    }
}