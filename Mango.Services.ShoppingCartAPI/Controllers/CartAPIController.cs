using AutoMapper;
using Mango.MessageBus;
using Mango.Services.ShoppingCartAPI.Data;
using Mango.Services.ShoppingCartAPI.Model;
using Mango.Services.ShoppingCartAPI.Model.Dto;
using Mango.Services.ShoppingCartAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Mango.Services.ShoppingCartAPI.Controllers
{
	[Route("api/cart")]
	[ApiController]
	public class CartAPiController : ControllerBase
	{
		private ResponseDto _response;
		private readonly AppDbContext _db;
		private IMapper _mapper;
		private readonly IProductServices _productServices;
		private readonly ICouponService _couponServices;
		private readonly IMessageBus _messageBus;
		private readonly IConfiguration _configuration;

		public CartAPiController(AppDbContext db, IMapper mapper, IProductServices productServices, 
			ICouponService couponService, IMessageBus messageBus, IConfiguration configuration)
		{
			_db = db;
			_mapper = mapper;
			_response = new ResponseDto();
			_productServices = productServices;
			_couponServices	= couponService;
			_messageBus = messageBus;
			_configuration = configuration;
		}

		[HttpGet("getcart/{userId}")]
		public async Task<ResponseDto> GetCart(string userId)
		{
			try
			{
				var cartHeaderEntity = await _db.CartHeaders.FirstOrDefaultAsync(a => a.UserId == userId);
				if (cartHeaderEntity == null)
				{
					_response.Result = new CartDto()
					{
						CartHeader = new CartHeaderDto(),
						CartDetails = new List<CartDetailsDto>()
					};
					return _response;
				}

				CartDto cart = new CartDto()
				{
					CartHeader = _mapper.Map<CartHeaderDto>(
						await _db.CartHeaders.FirstOrDefaultAsync(u => u.UserId == userId))
				};

				cart.CartDetails = _mapper.Map<IEnumerable<CartDetailsDto>>(
					_db.CartDetails.Where(u => u.CartHeaderId == cart.CartHeader.CartHeaderId))?? new List<CartDetailsDto>();

				IEnumerable<ProductDto> productDtos = await _productServices.GetProducts();


				foreach (var item in cart.CartDetails)
				{
					item.Product = productDtos.FirstOrDefault(u => u.ProductId == item.ProductId);
					cart.CartHeader.CartTotal += (item.Count * item.Product.Price);
					//item?.Product = productDtos.FirstOrDefault(u => u.ProductId == item.ProductId);
					//var product = productDtos.FirstOrDefault(u => u.ProductId == item.ProductId);
					//if (product == null)
					//	continue;
					//cart.CartHeader.CartTotal += (product.Price * item.Count);
				}

				if (!string.IsNullOrEmpty(cart.CartHeader.CouponCode))
				{
					CouponDto coupon = await _couponServices.GetCouponByCode(cart.CartHeader.CouponCode);
					if (coupon != null && cart.CartHeader.CartTotal > coupon.MinAmount )
					{
						cart.CartHeader.CartTotal -= coupon.DiscountAmount;
						cart.CartHeader.Discount = coupon.DiscountAmount;
					}
				}


				_response.Result = cart;
			}
			catch(Exception ex)
			{
				_response.Result = false;
				_response.Message = ex.Message;
			}
			return _response;
		}

		[HttpPost("applycoupon")]
		public async Task<ResponseDto> ApplyCoupon([FromBody] CartDto cartDto)
		{
			try
			{
				var cartFromDb = _db.CartHeaders.FirstOrDefault(a => a.UserId == cartDto.CartHeader.UserId);
				if (cartFromDb == null)
				{
					throw new Exception("Cart not found");	
				}
				cartFromDb.CouponCode = cartDto.CartHeader.CouponCode;
				_db.CartHeaders.Update(cartFromDb);
				await _db.SaveChangesAsync();
				_response.Result = true;
			}
			catch (Exception ex)
			{
				_response.isSuccess = false;
				_response.Message = ex.ToString();
			}
			return _response;
		}

		[HttpPost("emailcartrequest")]
		public async Task<ResponseDto> EmailCartRequest([FromBody] CartDto cartDto)
		{
			try
			{
				await _messageBus.PublishMessage(cartDto, _configuration.GetValue<string>("EmailShopingCartQueue:emailshoppingcart"));
				_response.Result = true;
			}
			catch (Exception ex)
			{
				_response.isSuccess = false;
				_response.Message = ex.ToString();
			}
			return _response;
		}

		[HttpPost("cartUpsert")]
		public async Task<ResponseDto> CartUpsert([FromBody] CartDto cartDto)
		{
			try
			{
				var cartHeaderFromDb = _db.CartHeaders.AsNoTracking().FirstOrDefault(u => u.UserId == cartDto.CartHeader.UserId);
				if (cartHeaderFromDb == null)
				{
					// create cart header and details
					CartHeader cartHeader = _mapper.Map<CartHeader>(cartDto.CartHeader);
					_db.CartHeaders.Add(cartHeader);
					await _db.SaveChangesAsync();
					var cartDetails = _mapper.Map<CartDetails>(cartDto.CartDetails.First());
					_db.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
					await _db.SaveChangesAsync();

				}
				else
				{
					// if cart header is not null
					// check if details has same product
					var cartDetailsFromDb = _db.CartDetails.AsNoTracking().FirstOrDefault(
						u => u.ProductId == cartDto.CartDetails.First().ProductId &&
						u.CartHeaderId == cartHeaderFromDb.CartHeaderId);

					if (cartDetailsFromDb == null)
					{
						// create cart details
						cartDto.CartDetails.First().CartHeaderId = cartHeaderFromDb.CartHeaderId;
						_db.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
						await _db.SaveChangesAsync();
					}
					else
					{
						// update the count / cart details
						cartDto.CartDetails.First().Count += cartDetailsFromDb.Count;
						cartDto.CartDetails.First().CartHeaderId = cartDetailsFromDb.CartHeaderId;
						cartDto.CartDetails.First().CartDetailsId = cartDetailsFromDb.CartDetailsId;
						_db.CartDetails.Update(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
						await _db.SaveChangesAsync();
					}

				}
				_response.Result = cartDto;
			}
			catch (Exception ex)
			{
				_response.isSuccess = false;
				_response.Message = ex.Message;
			}
			return _response;
		}

		[HttpPost("RemoveCart")]
		public async Task<ResponseDto> RemoveCart([FromBody]int cartDetailsId)
		{
			try
			{
				CartDetails cartDetails = await _db.CartDetails.FirstOrDefaultAsync(u => u.CartDetailsId == cartDetailsId);
				if (cartDetails == null)
				{
					throw new Exception("Cart Details not found");
				}

				int totalCountOfCartItems = _db.CartDetails.Where(u => u.CartHeaderId == cartDetails.CartHeaderId).Count();
				_db.CartDetails.Remove(cartDetails);
				if (totalCountOfCartItems == 1)
				{
					var cartHeaderToRemove = await _db.CartHeaders.FirstOrDefaultAsync(u => u.CartHeaderId == cartDetails.CartHeaderId);
					_db.CartHeaders.Remove(cartHeaderToRemove);
				}
				await _db.SaveChangesAsync();
				_response.Result = true;
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
