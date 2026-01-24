using Mango.Web.Model;
using Mango.Web.Models;

namespace Mango.Web.Services.IService
{
	public interface ICartService
	{
		Task<ResponseDto> GetCartByUserIdAsync(string userId);
		Task<ResponseDto> UpsertCartAsync(CartDto cartDto);
		Task<ResponseDto> RemoveFromCartAsync(int cartDetailsId);
		Task<ResponseDto> ApplyCouponAsync(CartDto cartDto);
	}
}
