using Mango.Services.AuthAPI.Data;
using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Models.Dto;
using Mango.Services.AuthAPI.Services.Iservice;
using Microsoft.AspNetCore.Identity;

namespace Mango.Services.AuthAPI.Services
{
	public class AuthService : IAuthService
	{
		private readonly AppDbContext _db;
		private readonly UserManager<ApplicationUser> _userManage;
		private readonly RoleManager<IdentityRole> _roleManger;
		private readonly IJwtTokenGenerator _jwtTokenGenerator;

		public AuthService(AppDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IJwtTokenGenerator jwtTokenGenerator)
		{
			_db = db;
			_userManage = userManager;
			_roleManger = roleManager;
			_jwtTokenGenerator = jwtTokenGenerator;
		}

		public async Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequestDto)
		{
			var user = _db.Users.FirstOrDefault(a => a.UserName.ToLower() == loginRequestDto.UserName.ToLower());

			bool isValid = await _userManage.CheckPasswordAsync(user, loginRequestDto.Password);

			if (user == null || isValid == false)
			{
				return new LoginResponseDto()
				{
					User = null,
					Token = ""
				};
			}

			// If user found generate JWT Token
			var roles = await _userManage.GetRolesAsync(user);
			var token = _jwtTokenGenerator.GenerateToken(user, roles);

			UserDto userDto = new()
			{
				Email = user.Email,
				Id = user.Id,
				Name = loginRequestDto.UserName,
				PhoneNumber = user.PhoneNumber
			};

			LoginResponseDto loginResponseDto = new LoginResponseDto()
			{
				User = userDto,
				Token = token
			};

			return loginResponseDto;
		}

		public async Task<string> RegisterAsync(RegistrationRequestDto registrationRequestDto)
		{
			ApplicationUser user = new()
			{
				UserName = registrationRequestDto.Email,
				Email = registrationRequestDto.Email,
				NormalizedEmail = registrationRequestDto.Email.ToUpper(),
				Name = registrationRequestDto.Name,
				PhoneNumber = registrationRequestDto.PhoneNumber,
			};

			try
			{
				var result = await _userManage.CreateAsync(user, registrationRequestDto.Password);
				if (result.Succeeded)
				{
					var userToReturn = _db.ApplicationUsers.First(u => u.UserName == registrationRequestDto.Email);
					UserDto userDto = new()
					{
						Id = userToReturn.Id,
						Name = userToReturn.Name,
						Email = userToReturn.Email,
						PhoneNumber = userToReturn.PhoneNumber,
					};
					return "";
				}
				else
				{
					return result.Errors.First().Description;
				}
			}
			catch (Exception)
			{
				throw;

			}

			return "ERROR ENCOUNTERED";
		}

		public async Task<bool> AssignRole(string email, string roleName)
		{
			var user = _db.Users.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
			if (user != null)
			{
				if (! _roleManger.RoleExistsAsync(roleName).GetAwaiter().GetResult())
				{
					// create a role if it doesn't exist
					var roleResult = await _roleManger.CreateAsync(new IdentityRole(roleName));
					if (!roleResult.Succeeded)
						return false;
				}
				await _userManage.AddToRoleAsync(user, roleName);
				return true;
			}
		    return false;
		}
	}
}
