using Mando.Web.Services;
using Mando.Web.Services.IService;
using Mango.Web.Model;
using Mango.Web.Services;
using Mango.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
	public class ProductController : Controller
	{
		private readonly IProductService _productService;
		private readonly IBaseService _baseService;

		public ProductController(IBaseService baseService, IProductService productService)
		{
			_baseService = baseService;
			_productService = productService;
		}

		public async Task<IActionResult> ProductIndex()
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

		public async Task<IActionResult> ProductCreate()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> ProductCreate(ProductDto model)
		{
			if (ModelState.IsValid)
			{
				ResponseDto? response = await _productService.CreateProductAsync(model);
				if (response != null && response.isSuccess)
				{
					TempData["Success"] = "Product Created Successfully";
					return RedirectToAction(nameof(ProductIndex));
				}
				else
				{
					TempData["Error"] = response?.Message;
				}
			}

			return View(model);
		}

		public async Task<IActionResult> ProductDelete(int productId)
		{
			ResponseDto? response = await _productService.GetProductsByIdAsync(productId);
			if (response != null && response.isSuccess)
			{
				ProductDto? model = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
				return View(model);
			}
			else
			{
				TempData["Error"] = response?.Message;
			}
			return NotFound();
		}

		[HttpPost]
		public async Task<IActionResult> ProductDelete(ProductDto productDto)
		{
			ResponseDto? response = await _productService.DeleteProductAsync(productDto.ProductId);
			if (response != null && response.isSuccess)
			{
				TempData["Success"] = "Product Deleted Successfully";
				return RedirectToAction(nameof(ProductIndex));
			}
			else
			{
				TempData["Error"] = response?.Message;
			}
			return View(productDto);
		}

		public async Task<IActionResult> ProductEdit(int productId)
		{
			ResponseDto? response = await _productService.GetProductsByIdAsync(productId);
			if (response != null && response.isSuccess)
			{
				ProductDto? model = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
				return View(model);
			}
			else
			{
				TempData["Error"] = response?.Message;
			}
			return NotFound();
		}

		[HttpPost]
		public async Task<IActionResult> ProductEdit(ProductDto productDto)
		{
			ResponseDto? response = await _productService.UpdateProductAsync(productDto);
			if (response != null && response.isSuccess)
			{
				TempData["Success"] = "Product Updated Successfully";
				return RedirectToAction(nameof(ProductIndex));
			}
			else
			{
				TempData["Error"] = response?.Message;
			}
			return View(productDto);
		}
	}
}
