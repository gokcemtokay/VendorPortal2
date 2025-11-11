using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using VendorPortal.Data;
using VendorPortal.Models.Entities;
using VendorPortal.Services;
using VendorPortal.Workers;

var builder = WebApplication.CreateBuilder(args);

// PostgreSQL bağlantısı
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity yapılandırması
builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// JWT Authentication yapılandırması
var jwtKey = builder.Configuration["Jwt:Key"] 
    ?? throw new InvalidOperationException("JWT Key is not configured");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero
    };
});

// Cookie Authentication (MVC için)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(24);
    options.SlidingExpiration = true;
});

// Services
builder.Services.AddScoped<ISiparisService, SiparisService>();
builder.Services.AddScoped<IFirmaService, FirmaService>();
builder.Services.AddScoped<IMalzemeService, MalzemeService>();
builder.Services.AddScoped<IIhaleService, IhaleService>();

// Background Workers
builder.Services.AddSingleton<GenericImportWorker>();
builder.Services.AddHostedService(provider => provider.GetRequiredService<GenericImportWorker>());

// MVC
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// CORS - React için
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactPolicy", builder =>
    {
        builder.WithOrigins("http://localhost:3000")
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
    });
});

// SPA Static Files
builder.Services.AddSpaStaticFiles(configuration =>
{
    configuration.RootPath = "ClientApp/build";
});

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Vendor Portal API",
        Version = "v1",
        Description = "Vendor Portal API Documentation"
    });

    // JWT için Swagger authorization
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Veritabanı oluşturma ve seed (development ortamında)
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        
        // Migrations uygula
        context.Database.Migrate();
        
        // Seed data
        await SeedData(context, userManager, roleManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Veritabanı oluşturulurken hata oluştu.");
    }
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Vendor Portal API V1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// SPA Static Files
app.UseSpaStaticFiles();

// CORS
app.UseCors("ReactPolicy");

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// SPA Configuration
app.UseSpa(spa =>
{
    spa.Options.SourcePath = "ClientApp";

    if (app.Environment.IsDevelopment())
    {
        spa.UseProxyToSpaDevelopmentServer("http://localhost:3000");
    }
});

app.Run();

// Seed Data Method
async Task SeedData(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager)
{
    // Rolleri oluştur
    string[] roles = { "Admin", "Musteri", "Tedarikci" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole<Guid> { Name = role });
        }
    }

    // Admin kullanıcı oluştur
    var adminEmail = "admin@vendorportal.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            FirstName = "Admin",
            LastName = "User",
            EmailConfirmed = true,
            IsActive = true
        };

        var result = await userManager.CreateAsync(adminUser, "Admin123!");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }

    // Örnek firmalar oluştur
    if (!context.Firmalar.Any())
    {
        var musteriFirma = new VendorPortal.Models.Entities.Firma
        {
            Id = Guid.NewGuid(),
            Ad = "Örnek Müşteri Firma A.Ş.",
            VergiNo = "1234567890",
            Email = "musteri@ornek.com",
            Telefon = "0555 123 4567",
            Adres = "İstanbul, Türkiye",
            FirmaTipi = VendorPortal.Models.Enums.FirmaTipi.Musteri,
            Durum = VendorPortal.Models.Enums.FirmaDurumu.Onaylandi,
            CreatedDate = DateTime.UtcNow
        };

        var tedarikciFirma = new VendorPortal.Models.Entities.Firma
        {
            Id = Guid.NewGuid(),
            Ad = "Örnek Tedarikçi Ltd. Şti.",
            VergiNo = "9876543210",
            Email = "tedarikci@ornek.com",
            Telefon = "0555 987 6543",
            Adres = "Ankara, Türkiye",
            FirmaTipi = VendorPortal.Models.Enums.FirmaTipi.Tedarikci,
            Durum = VendorPortal.Models.Enums.FirmaDurumu.Onaylandi,
            CreatedDate = DateTime.UtcNow
        };

        context.Firmalar.AddRange(musteriFirma, tedarikciFirma);
        await context.SaveChangesAsync();

        // Örnek malzemeler oluştur
        var malzeme1 = new VendorPortal.Models.Entities.Malzeme
        {
            Id = Guid.NewGuid(),
            FirmaId = tedarikciFirma.Id,
            Kod = "MLZ001",
            Ad = "Örnek Malzeme 1",
            Birim = "Adet",
            Fiyat = 100.50m,
            ParaBirimi = "TRY",
            CreatedDate = DateTime.UtcNow
        };

        var malzeme2 = new VendorPortal.Models.Entities.Malzeme
        {
            Id = Guid.NewGuid(),
            FirmaId = tedarikciFirma.Id,
            Kod = "MLZ002",
            Ad = "Örnek Malzeme 2",
            Birim = "KG",
            Fiyat = 250.75m,
            ParaBirimi = "TRY",
            CreatedDate = DateTime.UtcNow
        };

        context.Malzemeler.AddRange(malzeme1, malzeme2);
        await context.SaveChangesAsync();
    }
}
