using Mango.Services.ShoppingCartAPI.Models.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mango.Services.ShoppingCartAPI.Services.IServices
{
	public interface IProductServices
	{
		Task<IEnumerable<ProductDto>> GetProducts();
	}
}
