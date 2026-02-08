using Mango.Services.OrderAPI.Models.Dto;
using Mango.Services.OrderAPI.Services.IServices;
using Newtonsoft.Json;


namespace Mango.Services.OrderAPI.Services
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
