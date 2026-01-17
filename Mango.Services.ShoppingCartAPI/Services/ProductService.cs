using Mango.Services.ShoppingCartAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Services.IServices;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using Mango.Services.ShoppingCartAPI.Model.Dto;
using Newtonsoft.Json;


namespace Mango.Services.ShoppingCartAPI.Services
{
	public class ProductService : IProductServices
	{
		private readonly IHttpClientFactory _httpClientFactory;

		public ProductService(IHttpClientFactory httpClientFactory)
		{
			_httpClientFactory = httpClientFactory;
		}

		public async Task<IEnumerable<ProductDto>> GetProducts()
		{
			var client = _httpClientFactory.CreateClient("Product");
			var response = await client.GetAsync($"/api/product");

			var apiContent = await response.Content.ReadAsStringAsync();
			var res = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
			if (res != null && res.isSuccess)
			{
				var products = JsonConvert.DeserializeObject<IEnumerable<ProductDto>>(res.Result.ToString());
				return products!;
			}

			return new List<ProductDto>();

		}
	}
}
