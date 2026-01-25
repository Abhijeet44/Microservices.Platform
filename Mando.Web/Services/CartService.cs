using Mando.Web.Models;
using Mando.Web.Models;
using Mando.Web.Services;
using Mando.Web.Services.IService;
using Mango.Web.Model;
using Mango.Web.Model;
using Mango.Web.Models;
using Mango.Web.Services.IService;
using Mango.Web.Utility;
using System;
using static Mango.Web.Utility.SD;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Mango.Web.Services
{
	public class CartService : ICartService
	{
		private readonly IBaseService _baseService;

		public CartService(IBaseService baseService)
		{
			_baseService = baseService;
		}

		public async Task<ResponseDto> ApplyCouponAsync(CartDto cartDto)
		{
			return await _baseService.SendAsync(new RequestDto()
			{
				APIType = SD.ApiType.POST,
				Data = cartDto,
				URL = SD.ShoppingCartAPIBase + "/api/cart/applycoupon"
			});
		}

		public async Task<ResponseDto> EmailCart(CartDto cartDto)
		{
			return await _baseService.SendAsync(new RequestDto()
			{
				APIType = SD.ApiType.POST,
				Data = cartDto,
				URL = SD.ShoppingCartAPIBase + "/api/cart/emailcartrequest"
			});
		}

		public async Task<ResponseDto> GetCartByUserIdAsync(string userId)
		{
			return await _baseService.SendAsync(new RequestDto()
			{
				APIType = SD.ApiType.GET,
				URL = SD.ShoppingCartAPIBase + "/api/cart/getcart/" + userId
			});
		}

		public async Task<ResponseDto> RemoveFromCartAsync(int cartDetailsId)
		{
			return await _baseService.SendAsync(new RequestDto()
			{
				APIType = SD.ApiType.POST,
				Data = cartDetailsId,
				URL = SD.ShoppingCartAPIBase + "/api/cart/RemoveCart"
			});
		}

		public async Task<ResponseDto> UpsertCartAsync(CartDto cartDto)
		{
			return await _baseService.SendAsync(new RequestDto()
			{
				APIType = SD.ApiType.POST,
				Data = cartDto,
				URL = SD.ShoppingCartAPIBase + "/api/cart/cartUpsert"
			});
		}
	}
}
