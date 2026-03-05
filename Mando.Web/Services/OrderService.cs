using Mando.Web.Services.IService;
using Mango.Web.Model;
using Mango.Web.Models;
using Mango.Web.Services.IService;
using Mango.Web.Utility;
using Mando.Web.Models;

namespace Mango.Web.Services
{
	public class OrderService : IOrderService
	{
		private readonly IBaseService _baseService;
		public OrderService(IBaseService baseService)
		{
			_baseService = baseService;
		}
		public async Task<ResponseDto> CreateOrder(CartDto cartDto)
		{
		    return await _baseService.SendAsync(new RequestDto
			{
				APIType = SD.ApiType.POST,
				URL = SD.OrderAPIBase + "/api/order/createorder",
				Data = cartDto
			});
		}

		public async Task<ResponseDto> CreateStripeSession(StripeRequestDto stripeRequestDto)
		{
			return await _baseService.SendAsync(new RequestDto
			{
				APIType = SD.ApiType.POST,
				URL = SD.OrderAPIBase + "/api/order/CreateStripeSession",
				Data = stripeRequestDto
			});
		}

		public async Task<ResponseDto> GetAllOrders(string? userId)
		{
			return await _baseService.SendAsync(new RequestDto
			{
				APIType = SD.ApiType.GET,
				URL = SD.OrderAPIBase + "/api/order/getOrders",
				Data = userId
			});
		}

		public async Task<ResponseDto> GetOrder(int id)
		{
			return await _baseService.SendAsync(new RequestDto
			{
				APIType = SD.ApiType.GET,
				URL = SD.OrderAPIBase + "/api/order/getOrders/" + id,
				Data = id
			});
		}

		public async Task<ResponseDto> UpdateOrderStatus(int orderId, string newStatus)
		{
			return await _baseService.SendAsync(new RequestDto
			{
				APIType = SD.ApiType.POST,
				URL = SD.OrderAPIBase + "/api/order/updateOrderStatus/" + orderId,
				Data = newStatus
			});
		}

		public async Task<ResponseDto> ValidateStripeSession(int orderHeaderId)
		{
			return await _baseService.SendAsync(new RequestDto
			{
				APIType = SD.ApiType.POST,
				URL = SD.OrderAPIBase + "/api/order/ValidateStripeSession",
				Data = orderHeaderId
			});
		}
	}
}
