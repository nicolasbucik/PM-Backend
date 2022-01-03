using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PMan.AuthService.DTO.Incoming;
using PMan.AuthService.DTO.Outgoing;
using PMan.Core.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMan.AuthService.Services
{
    public interface IUserService
    {
        Task<DTOGenericOutgoingUserManangerResponse> RegisterUserAsync(DTOIncomingRegisterModel model);
        Task<DTOGenericOutgoingUserManangerResponse> LoginUserAsync(DTOIncomingLoginModel model);
    }

    public class UserService : IUserService
    {
        private UserManager<ApplicationUser> _userManager;
        private IConfiguration _configuration;

        public UserService(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _configuration = configuration;
            _userManager = userManager;

        }
        public async Task<DTOGenericOutgoingUserManangerResponse> RegisterUserAsync(DTOIncomingRegisterModel model)
        {
            if (model == null)
                throw new NullReferenceException("Register model is null");

            if (model.Password != model.ConfirmPassword)
                return new DTOOutgoingErrorUserManagerResponse
                {
                    Message = "Some errors occured during registration",
                    Errors = new List<string>() { "Provided passwords do not match" },
                };

            var identityUser = new ApplicationUser
            {
                Email = model.Email,
                UserName = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                
            };

            var result = await _userManager.CreateAsync(identityUser, model.Password);

            if (result.Succeeded)
                return new DTOGenericOutgoingUserManangerResponse
                {
                    Message = "User successufully created",
                    isSuccess = true
                };

            return new DTOOutgoingErrorUserManagerResponse
            {
                Message = "Errors occured when trying to create new user",
                Errors = result.Errors.Select(e => e.Description)
            };
        }
        public async Task<DTOGenericOutgoingUserManangerResponse> LoginUserAsync(DTOIncomingLoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
                return new DTOOutgoingErrorUserManagerResponse
                {
                    Message = "Errors occured when trying to log in",
                    Errors = new List<string>() { "Invalid credentials." },
                };
            var result = await _userManager.CheckPasswordAsync(user, model.Password);

            if (!result)
                return new DTOOutgoingErrorUserManagerResponse
                {
                    Message = "Errors occured when trying to log in",
                    Errors = new List<string>() { "Invalid credentials." },
                };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AuthSettings:Key"]));
            var token = new JwtSecurityToken(
                    issuer: _configuration["AuthSettings:Issuer"],
                    audience: _configuration["AuthSettings:Audience"],
                    expires: DateTime.Now.AddDays(30),
                    signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                );

            string tokenAsString = new JwtSecurityTokenHandler().WriteToken(token);

            return new DTOOutgoingSuccessLoginUserManagerResponse
            {
                Message = "Succesfully logged in",
                Token = tokenAsString,
                ExpireDate = token.ValidTo
            };
        }
    }
}
