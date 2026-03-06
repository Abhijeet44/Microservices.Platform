using Mango.Web.Model;
using Mango.Web.Models;
using Mango.Web.Services.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;

namespace Mango.Web.Controllers
{
	public class OrderController : Controller
	{
		private readonly IOrderService _orderService;

		public OrderController(IOrderService orderService)
		{
			_orderService = orderService;
		}

		public IActionResult OrderIndex()
		{
			return View();
		}

		public async Task<IActionResult> OrderDetail(int orderId)
		{
			OrderHeaderDto orderHeaderDto = new OrderHeaderDto();
			string userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
			var response = await _orderService.GetOrder(orderId);
			if (response != null && response.isSuccess)
			{
				orderHeaderDto = JsonConvert.DeserializeObject<OrderHeaderDto>(Convert.ToString(response.Result));
			}
			if (!User.IsInRole(SD.RoleAdmin) && userId != orderHeaderDto.UserId)
			{
				return NotFound();
			}

			return View(orderHeaderDto);
		}

		[HttpGet]
		public IActionResult GetAll(string status)
		{
			IEnumerable<OrderHeaderDto> list;
			string userId = "";
			if (!User.IsInRole(SD.RoleAdmin))
			{
				userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
			}
			ResponseDto response =  _orderService.GetAllOrders(userId).GetAwaiter().GetResult();
			if(response != null && response.isSuccess)
			{
				list = JsonConvert.DeserializeObject<List<OrderHeaderDto>>(Convert.ToString(response.Result));
				switch (status)
				{
					case "approved":
						list = list.Where(u => u.Status == SD.Status_Approved);
						break;
					case "readyforpickup":
						list = list.Where(u => u.Status == SD.Status_ReadyForPickup);
						break;
					case "cancelled":
						list = list.Where(u => u.Status == SD.Status_Cancelled);
						break;
					default:
						break;
				}
			}
			else
			{
				list = new List<OrderHeaderDto>();
			}
			return Json(new { data = list });
		}

		[HttpPost("OrderReadyForPickUp")]
		public async Task<IActionResult> OrderReadyForPickUp(int orderId)
		{
			var response = await _orderService.UpdateOrderStatus(orderId, SD.Status_ReadyForPickup);
			if (response != null && response.isSuccess)
			{
				TempData["success"] = "Status Updated Successfully.";
				return RedirectToAction(nameof(OrderIndex), new {orderId = orderId});
			}
			return View(orderId);
		}

		[HttpPost("CompleteOrder")]
		public async Task<IActionResult> CompleteOrder(int orderId)
		{
			var response = await _orderService.UpdateOrderStatus(orderId, SD.Status_Complete);
			if (response != null && response.isSuccess)
			{
				TempData["success"] = "Status Updated Successfully.";
				return RedirectToAction(nameof(OrderIndex), new { orderId = orderId });
			}
			return View(orderId);
		}

		[HttpPost("CancleOrder")]
		public async Task<IActionResult> CancleOrder(int orderId)
		{
			var response = await _orderService.UpdateOrderStatus(orderId, SD.Status_Cancelled);
			if (response != null && response.isSuccess)
			{
				TempData["success"] = "Status Updated Successfully.";
				return RedirectToAction(nameof(OrderIndex), new { orderId = orderId });
			}
			return View(orderId);
		}
	}
}
