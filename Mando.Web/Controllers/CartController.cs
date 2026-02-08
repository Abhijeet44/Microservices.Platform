using Mango.Web.Model;
using Mango.Web.Models;
using Mango.Web.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace Mango.Web.Controllers
{
	public class CartController : Controller
	{
		private readonly ICartService _cartService;
		private readonly IOrderService _orderService;

		public CartController(ICartService cartService, IOrderService orderService)
		{
			_cartService = cartService;
			_orderService = orderService;
		}

		[Authorize]
		public async Task<IActionResult> CartIndex()
		{
			return View(await LoadCartBasedOnLoggedInUser());
		}

		[Authorize]
		public async Task<IActionResult> Checkout()
		{
			return View(await LoadCartBasedOnLoggedInUser());
		}

		[HttpPost]
		[ActionName("Checkout")]
		public async Task<IActionResult> Checkout(CartDto cartDto)
		{
			CartDto cart = await LoadCartBasedOnLoggedInUser();

			cart.CartHeader.Name = cartDto.CartHeader.Name;
			cart.CartHeader.Email = cartDto.CartHeader.Email;
			cart.CartHeader.Phone = cartDto.CartHeader.Phone;

			var response = await _orderService.CreateOrder(cart);
			OrderHeaderDto orderHeaderDto = JsonConvert.DeserializeObject<OrderHeaderDto>(Convert.ToString(response.Result));
			
			if(response != null && response.isSuccess)
			{

			}
			return View();
		}

		public async Task<IActionResult> Remove(int cartDetailsId)
		{
			var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
			ResponseDto? response = await _cartService.RemoveFromCartAsync(cartDetailsId);
			if (response != null && response.isSuccess)
			{
				TempData["success"] = "Item removed from cart successfully.";
				return RedirectToAction(nameof(CartIndex));
			}
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> ApplyCoupon(CartDto cartDto)
		{
			ResponseDto? response = await _cartService.ApplyCouponAsync(cartDto);
			if (response != null && response.isSuccess)
			{
				TempData["success"] = "Cart updated successfully";
				return RedirectToAction(nameof(CartIndex));
			}
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> EmailCart(CartDto cartDto)
		{
			CartDto cart = await LoadCartBasedOnLoggedInUser();
			cart.CartHeader.Email = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Email)?.FirstOrDefault()?.Value;

			ResponseDto? response = await _cartService.EmailCart(cartDto);
			if (response != null && response.isSuccess)
			{
				TempData["success"] = "Email will be processed and sent successfully";
				return RedirectToAction(nameof(CartIndex));
			}
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> RemoveCoupon(CartDto cartDto)
		{
			cartDto.CartHeader.CouponCode = "";
			ResponseDto? response = await _cartService.ApplyCouponAsync(cartDto);
			if (response != null && response.isSuccess)
			{
				TempData["success"] = "Cart updated successfully";
				return RedirectToAction(nameof(CartIndex));
			}
			return View();
		}

		private async Task<CartDto> LoadCartBasedOnLoggedInUser()
		{
			var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
			ResponseDto? response = await _cartService.GetCartByUserIdAsync(userId);
			if (response != null && response.isSuccess)
			{
				CartDto cartDto = JsonConvert.DeserializeObject<CartDto>(Convert.ToString(response.Result));
				return cartDto;
			}
			return new CartDto();


		}
	}
}
