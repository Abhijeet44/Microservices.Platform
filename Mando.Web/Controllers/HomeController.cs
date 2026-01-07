using Mando.Web.Models;
using Mando.Web.Services.IService;
using Mango.Web.Model;
using Mango.Web.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Mando.Web.Controllers
{
	public class HomeController : Controller
	{
		private readonly IProductService _productService;
		private readonly IBaseService _baseService;

		public HomeController(IBaseService baseService, IProductService productService)
		{
			_baseService = baseService;
			_productService = productService;
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
