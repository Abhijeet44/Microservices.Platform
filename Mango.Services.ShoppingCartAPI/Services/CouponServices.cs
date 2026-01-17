using Mango.Services.ShoppingCartAPI.Model.Dto;
using Mango.Services.ShoppingCartAPI.Services.IServices;
using Newtonsoft.Json;
using System.Collections;
using System.Net.Http;
using System.Threading.Tasks;

namespace Mango.Services.ShoppingCartAPI.Services
{
	public class CouponServices : ICouponService
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly ResponseDto _response;

		public CouponServices(IHttpClientFactory httpClientFactory)
		{
			_httpClientFactory = httpClientFactory;
			_response = new ResponseDto();
		}
		public async Task<CouponDto> GetCouponByCode(string couponCode)
		{
			var client = _httpClientFactory.CreateClient("Coupon");
			var response = await client.GetAsync($"api/CouponAPI/GetByCoupon/{couponCode}"); 
			
			var apiContent = await response.Content.ReadAsStringAsync();
			var res = JsonConvert.DeserializeObject<ResponseDto>(apiContent);

			if (res != null && res.isSuccess)
			{
				var coupon = JsonConvert.DeserializeObject<CouponDto>(res.Result.ToString());
				return coupon;
			}
			return new CouponDto();

		}
	}
}
