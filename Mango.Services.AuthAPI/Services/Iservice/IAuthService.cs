using Mango.Services.AuthAPI.Models.Dto;

namespace Mango.Services.AuthAPI.Services.Iservice
{
	public interface IAuthService
	{
		Task<string> RegisterAsync(RegistrationRequestDto registrationRequestDto);

		Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequestDto);

		Task<bool> AssignRole(string email, string roleName);
	}
}
