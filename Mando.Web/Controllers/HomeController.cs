using Mando.Web.Models;
using Mando.Web.Services.IService;
using Mango.Web.Model;
using Mango.Web.Models;
using Mango.Web.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Mando.Web.Controllers
{
	public class HomeController : Controller
	{
		private readonly IProductService _productService;
		private readonly ICartService _cartService;
		private readonly IBaseService _baseService;

		public HomeController(IBaseService baseService, IProductService productService, ICartService cartService)
		{
			_baseService = baseService;
			_productService = productService;
			_cartService = cartService;
		}

		public async Task<IActionResult> Index()
		{
			List<ProductDto>? list = new();

			ResponseDto? response = await _productService.GetAllProductsAsync();

			if (response != null && response.isSuccess)
			{
				list = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));
			}
			else
			{
				TempData["Error"] = response?.Message;
			}

			return View(list);
		}

		[Authorize]
		public async Task<IActionResult> ProductDetails(int ProductId)
		{
			ProductDto? model = new();
			ResponseDto? response = await _productService.GetProductsByIdAsync(ProductId);

			if (response != null && response.isSuccess)
			{
				model = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
			}
			else
			{
				TempData["Error"] = response?.Message;
			}

			return View(model);
		}

		[Authorize]
		[HttpPost]
		[ActionName("ProductDetails")]
		public async Task<IActionResult> ProductDetails(ProductDto productDto)
		{
			CartDto cartDto = new()
			{
				CartHeader = new CartHeaderDto()
				{
					UserId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value,
				}
			};

			CartDetailsDto cartDetails = new CartDetailsDto()
			{
				Count = productDto.Count,
				ProductId = productDto.ProductId,

			};

			List<CartDetailsDto> cartDetailsDtos = new() { cartDetails };
			cartDto.CartDetails = cartDetailsDtos;

			ResponseDto? response = await _cartService.UpsertCartAsync(cartDto);

			if (response != null && response.isSuccess)
			{
				TempData["Success"] = "Product has been added to the cart";
				return RedirectToAction(nameof(Index));
			}
			else
			{
				TempData["Error"] = response?.Message;
			}

			return View(productDto);
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
