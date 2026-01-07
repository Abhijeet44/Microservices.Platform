using AutoMapper;
using Mango.Services.ProductAPI.Data;
using Mango.Services.ProductAPI.Model.Dto;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ProductAPI.Controllers
{
	[Route("api/product")]
	[ApiController]
	public class ProductAPIController : ControllerBase
	{
		private readonly AppDbContext _db;
		private readonly ResponseDto _response;
		private IMapper _mapper;

		public ProductAPIController(AppDbContext db, IMapper mapper)
		{
			_db = db;
			_response = new ResponseDto();
			_mapper = mapper;
		}

		[HttpGet]
		public ResponseDto GetProducts()
		{
			try
			{
				IEnumerable<Product> objList = _db.Products.ToList();
				_response.Result = _mapper.Map<IEnumerable<ProductDto>>(objList);
			}
			catch (Exception ex)
			{
				_response.isSuccess = false;
				_response.Message = ex.Message;
			}

			return _response;
		}

		[HttpGet]
		[Route("{id:int}")]
		public ResponseDto GetProductsById(int id)
		{
			try
			{
				Product obj = _db.Products.First(u => u.ProductId == id);
				_response.Result = _mapper.Map<ProductDto>(obj);
			}
			catch (Exception ex)
			{
				_response.isSuccess = false;
				_response.Message = ex.Message;
			}
			return _response;
		}

		[HttpPost]
		[Authorize(Roles = "Admin")]
		public ResponseDto Post([FromBody] ProductDto productDto)
		{
			try
			{
				Product obj = _mapper.Map<Product>(productDto);
				_db.Products.Add(obj);
				_db.SaveChanges();
				_response.Result = _mapper.Map<ProductDto>(obj);
			}
			catch (Exception ex)
			{
				_response.isSuccess = false;
				_response.Message = ex.Message;
			}
			return _response;
		}

		[HttpPut]
		[Authorize(Roles = "Admin")]
		public ResponseDto UpdateProduct([FromBody] ProductDto productDto)
		{
			try
			{
				Product obj = _mapper.Map<Product>(productDto);
				_db.Products.Update(obj);
				_db.SaveChanges();
				_response.Result = _mapper.Map<ProductDto>(obj);
			}
			catch (Exception ex)
			{
				_response.isSuccess = false;
				_response.Message = ex.Message;
			}
			return _response;
		}

		[HttpDelete]
		[Route("{id:int}")]
		[Authorize(Roles = "Admin")]
		public ResponseDto DeleteProduct(int id)
		{
			try
			{
				Product obj = _db.Products.First(u => u.ProductId == id);
				_db.Products.Remove(obj);
				_db.SaveChanges();
			}
			catch (Exception ex)
			{
				_response.isSuccess = false;
				_response.Message = ex.Message;
			}
			return _response;
		}
	}
}
