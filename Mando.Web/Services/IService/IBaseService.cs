using Mando.Web.Models;
using Mango.Web.Model;

namespace Mando.Web.Services.IService
{
	public interface IBaseService
	{
		Task<ResponseDto?> SendAsync(RequestDto requestDto, bool withBearer = true);
	}
}
