using Mango.Services.AuthAPI.Models;

namespace Mango.Services.AuthAPI.Services.Iservice
{
	public interface IJwtTokenGenerator
	{
		string GenerateToken(ApplicationUser applicationUser);
	}
}
