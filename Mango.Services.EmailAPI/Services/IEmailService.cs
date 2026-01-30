using Mango.Services.EmailAPI.Models.Dtos;

namespace Mango.Services.EmailAPI.Services
{
	public interface IEmailService
	{
		Task EmailCartAndLog(CartDto cartDto);
		Task RegisterdEmailAndLog(string email);
	}
}
