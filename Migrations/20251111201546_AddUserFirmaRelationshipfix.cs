using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace VendorPortal.Migrations
{
    /// <inheritdoc />
    public partial class AddUserFirmaRelationshipfix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MailBildirimleri",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AliciEmail = table.Column<string>(type: "text", nullable: false),
                    Konu = table.Column<string>(type: "text", nullable: false),
                    Icerik = table.Column<string>(type: "text", nullable: false),
                    GonderimTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Durum = table.Column<int>(type: "integer", nullable: false),
                    ReferansTipi = table.Column<int>(type: "integer", nullable: true),
                    ReferansId = table.Column<Guid>(type: "uuid", nullable: true),
                    HataMesaji = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailBildirimleri", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: true),
                    LastName = table.Column<string>(type: "text", nullable: true),
                    FirmaId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastLoginDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Firmalar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Ad = table.Column<string>(type: "text", nullable: false),
                    VergiNo = table.Column<string>(type: "text", nullable: true),
                    Adres = table.Column<string>(type: "text", nullable: true),
                    Telefon = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    WebSitesi = table.Column<string>(type: "text", nullable: true),
                    FirmaTipi = table.Column<int>(type: "integer", nullable: false),
                    Durum = table.Column<int>(type: "integer", nullable: false),
                    OnayTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Firmalar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Firmalar_AspNetUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Firmalar_AspNetUsers_ModifiedByUserId",
                        column: x => x.ModifiedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Dokumanlar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirmaId = table.Column<Guid>(type: "uuid", nullable: false),
                    Kategori = table.Column<int>(type: "integer", nullable: false),
                    Baslik = table.Column<string>(type: "text", nullable: false),
                    Aciklama = table.Column<string>(type: "text", nullable: true),
                    DosyaAdi = table.Column<string>(type: "text", nullable: false),
                    DosyaYolu = table.Column<string>(type: "text", nullable: false),
                    DosyaTipi = table.Column<string>(type: "text", nullable: true),
                    BoyutKB = table.Column<int>(type: "integer", nullable: false),
                    Versiyon = table.Column<string>(type: "text", nullable: true),
                    YukleyenUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    YuklemeTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GecerlilikBitisTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dokumanlar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dokumanlar_AspNetUsers_YukleyenUserId",
                        column: x => x.YukleyenUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Dokumanlar_Firmalar_FirmaId",
                        column: x => x.FirmaId,
                        principalTable: "Firmalar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FirmaIliskileri",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MusteriFirmaId = table.Column<Guid>(type: "uuid", nullable: false),
                    TedarikciFirmaId = table.Column<Guid>(type: "uuid", nullable: false),
                    Durum = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FirmaIliskileri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FirmaIliskileri_AspNetUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FirmaIliskileri_Firmalar_MusteriFirmaId",
                        column: x => x.MusteriFirmaId,
                        principalTable: "Firmalar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FirmaIliskileri_Firmalar_TedarikciFirmaId",
                        column: x => x.TedarikciFirmaId,
                        principalTable: "Firmalar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Ihaleler",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IhaleAdi = table.Column<string>(type: "text", nullable: false),
                    ReferansNo = table.Column<string>(type: "text", nullable: false),
                    IhaleTuru = table.Column<int>(type: "integer", nullable: false),
                    Aciklama = table.Column<string>(type: "text", nullable: true),
                    TeklifBaslangicTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TeklifBitisTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GecerlilikSuresiGun = table.Column<int>(type: "integer", nullable: false),
                    ParaBirimi = table.Column<string>(type: "text", nullable: false),
                    Durum = table.Column<int>(type: "integer", nullable: false),
                    HerkeseAcikMi = table.Column<bool>(type: "boolean", nullable: false),
                    TumTedarikcilereAcikMi = table.Column<bool>(type: "boolean", nullable: false),
                    MusteriFirmaId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ihaleler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ihaleler_AspNetUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Ihaleler_Firmalar_MusteriFirmaId",
                        column: x => x.MusteriFirmaId,
                        principalTable: "Firmalar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Malzemeler",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirmaId = table.Column<Guid>(type: "uuid", nullable: false),
                    Kod = table.Column<string>(type: "text", nullable: false),
                    Ad = table.Column<string>(type: "text", nullable: false),
                    Aciklama = table.Column<string>(type: "text", nullable: true),
                    Birim = table.Column<string>(type: "text", nullable: true),
                    Fiyat = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    ParaBirimi = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Malzemeler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Malzemeler_AspNetUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Malzemeler_Firmalar_FirmaId",
                        column: x => x.FirmaId,
                        principalTable: "Firmalar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SiparisBasliklar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReferansNo = table.Column<string>(type: "text", nullable: false),
                    SiparisTuru = table.Column<int>(type: "integer", nullable: false),
                    Aciklama = table.Column<string>(type: "text", nullable: true),
                    SiparisTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    HedefTeslimatTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MusteriFirmaId = table.Column<Guid>(type: "uuid", nullable: false),
                    TedarikciFirmaId = table.Column<Guid>(type: "uuid", nullable: false),
                    ParaBirimi = table.Column<string>(type: "text", nullable: false),
                    ToplamTutar = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Durum = table.Column<int>(type: "integer", nullable: false),
                    DokumanlarJson = table.Column<string>(type: "text", nullable: true),
                    TeslimatAdresi = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiparisBasliklar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SiparisBasliklar_AspNetUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SiparisBasliklar_AspNetUsers_ModifiedByUserId",
                        column: x => x.ModifiedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SiparisBasliklar_Firmalar_MusteriFirmaId",
                        column: x => x.MusteriFirmaId,
                        principalTable: "Firmalar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SiparisBasliklar_Firmalar_TedarikciFirmaId",
                        column: x => x.TedarikciFirmaId,
                        principalTable: "Firmalar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IhaleDavetleri",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IhaleId = table.Column<Guid>(type: "uuid", nullable: false),
                    TedarikciFirmaId = table.Column<Guid>(type: "uuid", nullable: false),
                    DavetTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IhaleDavetleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IhaleDavetleri_AspNetUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IhaleDavetleri_Firmalar_TedarikciFirmaId",
                        column: x => x.TedarikciFirmaId,
                        principalTable: "Firmalar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IhaleDavetleri_Ihaleler_IhaleId",
                        column: x => x.IhaleId,
                        principalTable: "Ihaleler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IhaleTeklifleri",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IhaleId = table.Column<Guid>(type: "uuid", nullable: false),
                    TedarikciFirmaId = table.Column<Guid>(type: "uuid", nullable: false),
                    TeklifNo = table.Column<string>(type: "text", nullable: false),
                    TeklifTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GecerlilikTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ToplamTeklifTutari = table.Column<decimal>(type: "numeric", nullable: false),
                    Durum = table.Column<int>(type: "integer", nullable: false),
                    Aciklama = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IhaleTeklifleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IhaleTeklifleri_AspNetUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IhaleTeklifleri_Firmalar_TedarikciFirmaId",
                        column: x => x.TedarikciFirmaId,
                        principalTable: "Firmalar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IhaleTeklifleri_Ihaleler_IhaleId",
                        column: x => x.IhaleId,
                        principalTable: "Ihaleler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IhaleKalemleri",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IhaleId = table.Column<Guid>(type: "uuid", nullable: false),
                    MalzemeId = table.Column<Guid>(type: "uuid", nullable: false),
                    MalzemeKodu = table.Column<string>(type: "text", nullable: false),
                    MalzemeAdi = table.Column<string>(type: "text", nullable: false),
                    IstenenMiktar = table.Column<decimal>(type: "numeric", nullable: false),
                    Birim = table.Column<string>(type: "text", nullable: false),
                    Spesifikasyon = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IhaleKalemleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IhaleKalemleri_AspNetUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IhaleKalemleri_Ihaleler_IhaleId",
                        column: x => x.IhaleId,
                        principalTable: "Ihaleler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IhaleKalemleri_Malzemeler_MalzemeId",
                        column: x => x.MalzemeId,
                        principalTable: "Malzemeler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TedarikciMalzemeEslestirmeleri",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MusteriFirmaId = table.Column<Guid>(type: "uuid", nullable: false),
                    MusteriMalzemeId = table.Column<Guid>(type: "uuid", nullable: false),
                    TedarikciFirmaId = table.Column<Guid>(type: "uuid", nullable: false),
                    TedarikciMalzemeId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TedarikciMalzemeEslestirmeleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TedarikciMalzemeEslestirmeleri_AspNetUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TedarikciMalzemeEslestirmeleri_Firmalar_MusteriFirmaId",
                        column: x => x.MusteriFirmaId,
                        principalTable: "Firmalar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TedarikciMalzemeEslestirmeleri_Firmalar_TedarikciFirmaId",
                        column: x => x.TedarikciFirmaId,
                        principalTable: "Firmalar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TedarikciMalzemeEslestirmeleri_Malzemeler_MusteriMalzemeId",
                        column: x => x.MusteriMalzemeId,
                        principalTable: "Malzemeler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TedarikciMalzemeEslestirmeleri_Malzemeler_TedarikciMalzemeId",
                        column: x => x.TedarikciMalzemeId,
                        principalTable: "Malzemeler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SiparisKalemler",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SiparisBaslikId = table.Column<Guid>(type: "uuid", nullable: false),
                    MalzemeId = table.Column<Guid>(type: "uuid", nullable: false),
                    MalzemeKodu = table.Column<string>(type: "text", nullable: false),
                    MalzemeAdi = table.Column<string>(type: "text", nullable: false),
                    Miktar = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Birim = table.Column<string>(type: "text", nullable: false),
                    BirimFiyat = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ToplamFiyat = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Aciklama = table.Column<string>(type: "text", nullable: true),
                    IstenenTeslimatTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    OnaylananMiktar = table.Column<decimal>(type: "numeric", nullable: true),
                    OnaylananBirimFiyat = table.Column<decimal>(type: "numeric", nullable: true),
                    Durum = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiparisKalemler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SiparisKalemler_AspNetUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SiparisKalemler_Malzemeler_MalzemeId",
                        column: x => x.MalzemeId,
                        principalTable: "Malzemeler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SiparisKalemler_SiparisBasliklar_SiparisBaslikId",
                        column: x => x.SiparisBaslikId,
                        principalTable: "SiparisBasliklar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IhaleDokumanlari",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IhaleId = table.Column<Guid>(type: "uuid", nullable: true),
                    IhaleTeklifId = table.Column<Guid>(type: "uuid", nullable: true),
                    DosyaAdi = table.Column<string>(type: "text", nullable: false),
                    DosyaYolu = table.Column<string>(type: "text", nullable: false),
                    DosyaTipi = table.Column<string>(type: "text", nullable: true),
                    BoyutKB = table.Column<int>(type: "integer", nullable: false),
                    YukleyenUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    YuklemeTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Aciklama = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IhaleDokumanlari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IhaleDokumanlari_AspNetUsers_YukleyenUserId",
                        column: x => x.YukleyenUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IhaleDokumanlari_IhaleTeklifleri_IhaleTeklifId",
                        column: x => x.IhaleTeklifId,
                        principalTable: "IhaleTeklifleri",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IhaleDokumanlari_Ihaleler_IhaleId",
                        column: x => x.IhaleId,
                        principalTable: "Ihaleler",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "IhaleTeklifKalemleri",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IhaleTeklifId = table.Column<Guid>(type: "uuid", nullable: false),
                    IhaleKalemId = table.Column<Guid>(type: "uuid", nullable: false),
                    TeklifEdilenMiktar = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Birim = table.Column<string>(type: "text", nullable: false),
                    BirimFiyat = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ToplamFiyat = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    IndirimOrani = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    TeslimSuresiGun = table.Column<int>(type: "integer", nullable: true),
                    Notlar = table.Column<string>(type: "text", nullable: true),
                    Durum = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IhaleTeklifKalemleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IhaleTeklifKalemleri_AspNetUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IhaleTeklifKalemleri_IhaleKalemleri_IhaleKalemId",
                        column: x => x.IhaleKalemId,
                        principalTable: "IhaleKalemleri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IhaleTeklifKalemleri_IhaleTeklifleri_IhaleTeklifId",
                        column: x => x.IhaleTeklifId,
                        principalTable: "IhaleTeklifleri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SiparisOnayGecmisleri",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SiparisBaslikId = table.Column<Guid>(type: "uuid", nullable: true),
                    SiparisKalemId = table.Column<Guid>(type: "uuid", nullable: true),
                    IslemTipi = table.Column<int>(type: "integer", nullable: false),
                    Aciklama = table.Column<string>(type: "text", nullable: true),
                    EskiDurum = table.Column<int>(type: "integer", nullable: true),
                    YeniDurum = table.Column<int>(type: "integer", nullable: true),
                    IslemYapanUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    IslemTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiparisOnayGecmisleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SiparisOnayGecmisleri_AspNetUsers_IslemYapanUserId",
                        column: x => x.IslemYapanUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SiparisOnayGecmisleri_SiparisBasliklar_SiparisBaslikId",
                        column: x => x.SiparisBaslikId,
                        principalTable: "SiparisBasliklar",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SiparisOnayGecmisleri_SiparisKalemler_SiparisKalemId",
                        column: x => x.SiparisKalemId,
                        principalTable: "SiparisKalemler",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_FirmaId",
                table: "AspNetUsers",
                column: "FirmaId");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dokumanlar_FirmaId",
                table: "Dokumanlar",
                column: "FirmaId");

            migrationBuilder.CreateIndex(
                name: "IX_Dokumanlar_YukleyenUserId",
                table: "Dokumanlar",
                column: "YukleyenUserId");

            migrationBuilder.CreateIndex(
                name: "IX_FirmaIliskileri_CreatedByUserId",
                table: "FirmaIliskileri",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_FirmaIliskileri_MusteriFirmaId",
                table: "FirmaIliskileri",
                column: "MusteriFirmaId");

            migrationBuilder.CreateIndex(
                name: "IX_FirmaIliskileri_TedarikciFirmaId",
                table: "FirmaIliskileri",
                column: "TedarikciFirmaId");

            migrationBuilder.CreateIndex(
                name: "IX_Firmalar_CreatedByUserId",
                table: "Firmalar",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Firmalar_ModifiedByUserId",
                table: "Firmalar",
                column: "ModifiedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Firmalar_VergiNo",
                table: "Firmalar",
                column: "VergiNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IhaleDavetleri_CreatedByUserId",
                table: "IhaleDavetleri",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_IhaleDavetleri_IhaleId",
                table: "IhaleDavetleri",
                column: "IhaleId");

            migrationBuilder.CreateIndex(
                name: "IX_IhaleDavetleri_TedarikciFirmaId",
                table: "IhaleDavetleri",
                column: "TedarikciFirmaId");

            migrationBuilder.CreateIndex(
                name: "IX_IhaleDokumanlari_IhaleId",
                table: "IhaleDokumanlari",
                column: "IhaleId");

            migrationBuilder.CreateIndex(
                name: "IX_IhaleDokumanlari_IhaleTeklifId",
                table: "IhaleDokumanlari",
                column: "IhaleTeklifId");

            migrationBuilder.CreateIndex(
                name: "IX_IhaleDokumanlari_YukleyenUserId",
                table: "IhaleDokumanlari",
                column: "YukleyenUserId");

            migrationBuilder.CreateIndex(
                name: "IX_IhaleKalemleri_CreatedByUserId",
                table: "IhaleKalemleri",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_IhaleKalemleri_IhaleId",
                table: "IhaleKalemleri",
                column: "IhaleId");

            migrationBuilder.CreateIndex(
                name: "IX_IhaleKalemleri_MalzemeId",
                table: "IhaleKalemleri",
                column: "MalzemeId");

            migrationBuilder.CreateIndex(
                name: "IX_Ihaleler_CreatedByUserId",
                table: "Ihaleler",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Ihaleler_MusteriFirmaId",
                table: "Ihaleler",
                column: "MusteriFirmaId");

            migrationBuilder.CreateIndex(
                name: "IX_Ihaleler_ReferansNo",
                table: "Ihaleler",
                column: "ReferansNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IhaleTeklifKalemleri_CreatedByUserId",
                table: "IhaleTeklifKalemleri",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_IhaleTeklifKalemleri_IhaleKalemId",
                table: "IhaleTeklifKalemleri",
                column: "IhaleKalemId");

            migrationBuilder.CreateIndex(
                name: "IX_IhaleTeklifKalemleri_IhaleTeklifId",
                table: "IhaleTeklifKalemleri",
                column: "IhaleTeklifId");

            migrationBuilder.CreateIndex(
                name: "IX_IhaleTeklifleri_CreatedByUserId",
                table: "IhaleTeklifleri",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_IhaleTeklifleri_IhaleId",
                table: "IhaleTeklifleri",
                column: "IhaleId");

            migrationBuilder.CreateIndex(
                name: "IX_IhaleTeklifleri_TedarikciFirmaId",
                table: "IhaleTeklifleri",
                column: "TedarikciFirmaId");

            migrationBuilder.CreateIndex(
                name: "IX_IhaleTeklifleri_TeklifNo",
                table: "IhaleTeklifleri",
                column: "TeklifNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Malzemeler_CreatedByUserId",
                table: "Malzemeler",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Malzemeler_FirmaId",
                table: "Malzemeler",
                column: "FirmaId");

            migrationBuilder.CreateIndex(
                name: "IX_SiparisBasliklar_CreatedByUserId",
                table: "SiparisBasliklar",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SiparisBasliklar_ModifiedByUserId",
                table: "SiparisBasliklar",
                column: "ModifiedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SiparisBasliklar_MusteriFirmaId",
                table: "SiparisBasliklar",
                column: "MusteriFirmaId");

            migrationBuilder.CreateIndex(
                name: "IX_SiparisBasliklar_ReferansNo",
                table: "SiparisBasliklar",
                column: "ReferansNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SiparisBasliklar_TedarikciFirmaId",
                table: "SiparisBasliklar",
                column: "TedarikciFirmaId");

            migrationBuilder.CreateIndex(
                name: "IX_SiparisKalemler_CreatedByUserId",
                table: "SiparisKalemler",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SiparisKalemler_MalzemeId",
                table: "SiparisKalemler",
                column: "MalzemeId");

            migrationBuilder.CreateIndex(
                name: "IX_SiparisKalemler_SiparisBaslikId",
                table: "SiparisKalemler",
                column: "SiparisBaslikId");

            migrationBuilder.CreateIndex(
                name: "IX_SiparisOnayGecmisleri_IslemYapanUserId",
                table: "SiparisOnayGecmisleri",
                column: "IslemYapanUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SiparisOnayGecmisleri_SiparisBaslikId",
                table: "SiparisOnayGecmisleri",
                column: "SiparisBaslikId");

            migrationBuilder.CreateIndex(
                name: "IX_SiparisOnayGecmisleri_SiparisKalemId",
                table: "SiparisOnayGecmisleri",
                column: "SiparisKalemId");

            migrationBuilder.CreateIndex(
                name: "IX_TedarikciMalzemeEslestirmeleri_CreatedByUserId",
                table: "TedarikciMalzemeEslestirmeleri",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TedarikciMalzemeEslestirmeleri_MusteriFirmaId",
                table: "TedarikciMalzemeEslestirmeleri",
                column: "MusteriFirmaId");

            migrationBuilder.CreateIndex(
                name: "IX_TedarikciMalzemeEslestirmeleri_MusteriMalzemeId",
                table: "TedarikciMalzemeEslestirmeleri",
                column: "MusteriMalzemeId");

            migrationBuilder.CreateIndex(
                name: "IX_TedarikciMalzemeEslestirmeleri_TedarikciFirmaId",
                table: "TedarikciMalzemeEslestirmeleri",
                column: "TedarikciFirmaId");

            migrationBuilder.CreateIndex(
                name: "IX_TedarikciMalzemeEslestirmeleri_TedarikciMalzemeId",
                table: "TedarikciMalzemeEslestirmeleri",
                column: "TedarikciMalzemeId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Firmalar_FirmaId",
                table: "AspNetUsers",
                column: "FirmaId",
                principalTable: "Firmalar",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Firmalar_AspNetUsers_CreatedByUserId",
                table: "Firmalar");

            migrationBuilder.DropForeignKey(
                name: "FK_Firmalar_AspNetUsers_ModifiedByUserId",
                table: "Firmalar");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Dokumanlar");

            migrationBuilder.DropTable(
                name: "FirmaIliskileri");

            migrationBuilder.DropTable(
                name: "IhaleDavetleri");

            migrationBuilder.DropTable(
                name: "IhaleDokumanlari");

            migrationBuilder.DropTable(
                name: "IhaleTeklifKalemleri");

            migrationBuilder.DropTable(
                name: "MailBildirimleri");

            migrationBuilder.DropTable(
                name: "SiparisOnayGecmisleri");

            migrationBuilder.DropTable(
                name: "TedarikciMalzemeEslestirmeleri");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "IhaleKalemleri");

            migrationBuilder.DropTable(
                name: "IhaleTeklifleri");

            migrationBuilder.DropTable(
                name: "SiparisKalemler");

            migrationBuilder.DropTable(
                name: "Ihaleler");

            migrationBuilder.DropTable(
                name: "Malzemeler");

            migrationBuilder.DropTable(
                name: "SiparisBasliklar");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Firmalar");
        }
    }
}
