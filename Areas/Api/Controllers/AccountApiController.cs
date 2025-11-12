using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using VendorPortal.Models.DTOs;
using VendorPortal.Models.Entities;

namespace VendorPortal.Areas.Api.Controllers
{
    [Area("Api")]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountApiController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AccountApiController> _logger;

        public AccountApiController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
            ILogger<AccountApiController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Login endpoint - JWT token döner
        /// </summary>
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Geçersiz giriş bilgileri" });
                }

                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    _logger.LogWarning($"Login failed: User not found - {model.Email}");
                    return Unauthorized(new { message = "Email veya şifre hatalı" });
                }

                if (!user.IsActive)
                {
                    _logger.LogWarning($"Login failed: User is inactive - {model.Email}");
                    return Unauthorized(new { message = "Hesabınız aktif değil" });
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
                if (!result.Succeeded)
                {
                    _logger.LogWarning($"Login failed: Invalid password - {model.Email}");
                    return Unauthorized(new { message = "Email veya şifre hatalı" });
                }

                // JWT Token oluştur
                var token = await GenerateJwtToken(user);

                _logger.LogInformation($"Login successful: {model.Email}");

                return Ok(new
                {
                    token = token,
                    user = new
                    {
                        id = user.Id,
                        email = user.Email,
                        firstName = user.FirstName,
                        lastName = user.LastName,
                        firmaId = user.FirmaId,
                        roles = await _userManager.GetRolesAsync(user)
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login error occurred");
                return StatusCode(500, new { message = "Giriş sırasında bir hata oluştu" });
            }
        }

        /// <summary>
        /// JWT token oluşturur
        /// </summary>
        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Rolleri ekle
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // FirmaId varsa ekle
            if (user.FirmaId.HasValue)
            {
                claims.Add(new Claim("FirmaId", user.FirmaId.Value.ToString()));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured")));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Register endpoint (opsiyonel)
        /// </summary>
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    IsActive = true,
                    EmailConfirmed = false
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Varsayılan rol atama (opsiyonel)
                    // await _userManager.AddToRoleAsync(user, "Musteri");

                    _logger.LogInformation($"User registered: {model.Email}");
                    return Ok(new { message = "Kayıt başarılı" });
                }

                return BadRequest(new { message = "Kayıt başarısız", errors = result.Errors });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Register error occurred");
                return StatusCode(500, new { message = "Kayıt sırasında bir hata oluştu" });
            }
        }
    }
}