using Mango.Web.Model;

namespace Mando.Web.Services.IService
{
	public interface ICouponService
	{
		Task<ResponseDto?> GetCouponAsync(string couponCode);
		Task<ResponseDto?> GetAllCouponsAsync();
		Task<ResponseDto?> GetCouponsByIdAsync(int id);
		Task<ResponseDto?> CreateCouponAsync(CouponDto couponDto);
		Task<ResponseDto?> UpdateCouponAsync(CouponDto couponDto);
		Task<ResponseDto?> DeleteCouponAsync(int id);
	}
}
