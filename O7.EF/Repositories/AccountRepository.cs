using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using O7.Core.Interfaces;
using O7.Core.Models;
using O7.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace O7.EF.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IMailingServices _mailingServices;

        // Constructor:
        public AccountRepository(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IMailingServices mailingServices)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _mailingServices = mailingServices;
        }

        // Login By Email or UserName:
        public async Task<LoginResponseViewModel> Login(LoginViewModel model)
        {
            LoginResponseViewModel response = new();

            ApplicationUser user = await _userManager.FindByEmailAsync(model.UserName) ?? await _userManager.FindByNameAsync(model.UserName);
            if(user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                response.Message = "UserName Or Password is incorrect !!";
                response.IsAuthenticated = false;
                return response;
            }
            else
            {
                var jwtToken = await CreateJwtToken(user);
                var roles = await _userManager.GetRolesAsync(user);

                response.IsAuthenticated = true;
                response.AccessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);
                response.UserName = user.UserName;
                response.Email = user.Email;
                response.Roles = roles.ToList();
                response.ExpireOn = jwtToken.ValidTo;

                if(user.RefreshTokens.Any(t => t.IsActive))
                {
                    var activeToken = user.RefreshTokens.FirstOrDefault(t => t.IsActive);
                    response.RefreshToken = activeToken.Token;
                    response.RefreshTokenExpiration = activeToken.ExpiresOn;
                }
                else
                {
                    var refreshToken = GenerateRefreshToken();
                    response.RefreshToken = refreshToken.Token;
                    response.RefreshTokenExpiration = refreshToken.ExpiresOn;
                    user.RefreshTokens.Add(refreshToken);

                    await _userManager.UpdateAsync(user);
                }

                return response;

            }
           
        }

        // Still Need Roles:
        public async Task<LoginResponseViewModel> Register(RegisterViewModel model)
        {
            LoginResponseViewModel response = new();

            // Check If UserName Of Email Is Exist:
            if (await _userManager.FindByEmailAsync(model.Email) != null)
            {
                response.Message = "Email Is Aleady Registered !";
                response.IsAuthenticated = false;
                return response;
            }
            else if(await _userManager.FindByNameAsync(model.UserName) != null)
            {
                response.Message = "UserName Is Aleardy Registered!";
                response.IsAuthenticated = false;
                return response;
            }
            else
            {
                ApplicationUser user = new ApplicationUser
                {
                    Email = model.Email,
                    UserName = model.UserName,
                    FirstName = model.FristName,
                    LastName = model.LastName,
                    IsActive = true
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    user.BusinessId = user.Id;
                    await _userManager.AddToRoleAsync(user, "Admin");

                    // Email Confirmation:

                    var refreshToken = GenerateRefreshToken();
                    user.RefreshTokens.Add(refreshToken);

                    await _userManager.UpdateAsync(user);

                    var jwtToken = await CreateJwtToken(user);
                    var roles = await _userManager.GetRolesAsync(user);

                    response.Roles = roles.ToList();
                    response.Email = model.Email;
                    response.UserName = model.UserName;
                    response.IsAuthenticated = true;
                    response.AccessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);
                    response.RefreshToken = refreshToken.Token;
                    response.RefreshTokenExpiration = refreshToken.ExpiresOn;
                    response.ExpireOn = jwtToken.ValidTo;

                    return response;
                }
                else
                {
                    var errors = string.Empty;
                    foreach(var error in result.Errors)
                    {
                        errors += $"{error.Description}, ";
                    }
                    response.IsAuthenticated = false;
                    response.Message = errors;
                    return response;
                }
            }

        }

        // To Create New Refresh Token:
        public async Task<LoginResponseViewModel> RefreshToken(string token)
        {
            var response = new LoginResponseViewModel();

            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

            if (user == null)
            {
                response.IsAuthenticated = false;
                response.Message = "Invalid Token";
                return response;
            }
            else
            {
                var refreshToken = user.RefreshTokens.Single(t => t.Token == token);

                if (!refreshToken.IsActive)
                {
                    response.IsAuthenticated = false;
                    response.Message = "Invalid Token";
                    return response;
                }

                refreshToken.RevokedOn = DateTime.UtcNow;

                var newRefreshToken = GenerateRefreshToken();
                user.RefreshTokens.Add(newRefreshToken);
                await _userManager.UpdateAsync(user);

                var jwtToken = await CreateJwtToken(user);
                var roles = await _userManager.GetRolesAsync(user);

                response.Email = user.Email;
                response.UserName = user.UserName;
                response.Roles = roles.ToList();
                response.IsAuthenticated = true;
                response.AccessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);
                response.RefreshToken = newRefreshToken.Token;
                response.RefreshTokenExpiration = newRefreshToken.ExpiresOn;

                return response;
            }

        }

        // To Revoke Refresh Token:
        public async Task<bool> RevokeToken(string token)
        {
            // Search for user:
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

            if (user != null)
            {
                var refreshToken = user.RefreshTokens.Single(t => t.Token == token);
                if (refreshToken.IsActive)
                {
                    refreshToken.RevokedOn = DateTime.UtcNow;
                    await _userManager.UpdateAsync(user);
                    return true;
                }
                return false;
            }
            return false;

        }



        #region Helper:

        #region TOKENS:

        // This Function To Create Tokens:
        private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();


            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            var Claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("id", user.Id),
                new Claim("bid", user.BusinessId)

            }.Union(userClaims).Union(roleClaims);

            var SymmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Authentication:JWT:Secret"]));
            var signingCredentials = new SigningCredentials(SymmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken
            (
                issuer: _configuration["Authentication:JWT:ValidIssuer"],
                audience: _configuration["Authentication:JWT:ValidAudience"],
                claims: Claims,
                expires: DateTime.Now.AddDays(Convert.ToDouble(_configuration["Authentication:JWT:DuraionInDays"])),
                signingCredentials: signingCredentials
            );

            return token;
        }

        // Finish : 
        private Core.Models.RefreshToken GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var generator = new RNGCryptoServiceProvider();
            generator.GetBytes(randomNumber);
            return new Core.Models.RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                ExpiresOn = DateTime.UtcNow.AddDays(10),
                CreatedOn = DateTime.UtcNow
            };

        }

        #endregion

        #endregion
    }
}
