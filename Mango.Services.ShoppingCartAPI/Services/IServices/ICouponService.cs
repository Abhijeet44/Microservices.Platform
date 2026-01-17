using Mango.Services.ShoppingCartAPI.Model.Dto;
using System.Threading.Tasks;

namespace Mango.Services.ShoppingCartAPI.Services.IServices
{
	public interface ICouponService
	{
		Task<CouponDto> GetCouponByCode(string couponCode);
	}
}
