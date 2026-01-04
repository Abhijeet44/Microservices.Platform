using Mando.Web.Models;
using Mando.Web.Services.IService;
using Mango.Web.Utility;
using Mango.Web.Model;

namespace Mango.Web.Services
{
	public class CouponService : ICouponService
	{
		private readonly IBaseService _baseService;

		public CouponService(IBaseService baseService)
		{
			_baseService = baseService;
		}

		public async Task<ResponseDto?> CreateCouponAsync(CouponDto couponDto)
		{
			return await _baseService.SendAsync(new RequestDto()
			{
				APIType = SD.ApiType.POST,
				URL = SD.CouponAPIBase + "/api/couponAPI",
				Data = couponDto
			});
		}

		public async Task<ResponseDto?> DeleteCouponAsync(int id)
		{
			return await _baseService.SendAsync(new RequestDto()
			{
				APIType = SD.ApiType.DELETE,
				URL = SD.CouponAPIBase + "/api/couponAPI/" + id
			});
		}

		public async Task<ResponseDto?> GetAllCouponsAsync()
		{
			return await _baseService.SendAsync(new RequestDto()
			{
				APIType = SD.ApiType.GET,
				URL = SD.CouponAPIBase + "/api/couponAPI"
			});
		}

		public async Task<ResponseDto?> GetCouponAsync(string couponCode)
		{
			return await _baseService.SendAsync(new RequestDto()
			{
				APIType = SD.ApiType.GET,
				URL = SD.CouponAPIBase + "/api/couponAPI/GetByCoupon/" + couponCode
			});
		}

		public async Task<ResponseDto?> GetCouponsByIdAsync(int id)
		{
			return await _baseService.SendAsync(new RequestDto()
			{
				APIType = SD.ApiType.GET,
				URL = SD.CouponAPIBase + "/api/couponAPI/" + id
			});
		}

		public async Task<ResponseDto?> UpdateCouponAsync(CouponDto couponDto)
		{
			return await _baseService.SendAsync(new RequestDto()
			{
				APIType = SD.ApiType.PUT,
				URL = SD.CouponAPIBase + "/api/couponAPI",
				Data = couponDto
			});
		}
	}
}
