using Mando.Web.Models;
using Mando.Web.Services.IService;
using Mango.Web.Model;
using Mango.Web.Services.IService;
using Mango.Web.Utility;

namespace Mango.Web.Services
{
	public class ProductService : IProductService
	{
		private readonly IBaseService _baseService;

		public ProductService(IBaseService baseService)
		{
			_baseService = baseService;
		}

		public async Task<ResponseDto?> CreateProductAsync(ProductDto productDto)
		{
			return await _baseService.SendAsync(new RequestDto()
			{
				APIType = SD.ApiType.POST,
				URL = SD.ProductAPIBase + "/api/product",
				Data = productDto
			});
		}

		public async Task<ResponseDto?> DeleteProductAsync(int id)
		{
			return await _baseService.SendAsync(new RequestDto()
			{
				APIType = SD.ApiType.DELETE,
				URL = SD.ProductAPIBase + "/api/product/" + id,
			});
		}

		public async Task<ResponseDto?> GetAllProductsAsync()
		{
			return await _baseService.SendAsync(new RequestDto()
			{
				APIType = SD.ApiType.GET,
				URL = SD.ProductAPIBase + "/api/product"
			});
		}

		public async Task<ResponseDto?> GetProductsByIdAsync(int id)
		{
			return await _baseService.SendAsync(new RequestDto()
			{
				APIType = SD.ApiType.GET,
				URL = SD.ProductAPIBase + "/api/product/" + id
			});
		}

		public async Task<ResponseDto?> UpdateProductAsync(ProductDto productDto)
		{
			return await _baseService.SendAsync(new RequestDto()
			{
				APIType = SD.ApiType.PUT,
				URL = SD.ProductAPIBase + "/api/product",
				Data = productDto
			});
		}
	}
}
