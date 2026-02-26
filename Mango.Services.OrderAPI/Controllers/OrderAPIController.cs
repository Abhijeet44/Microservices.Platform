using AutoMapper;
using Mango.Services.OrderAPI.Data;
using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Models.Dto;
using Mango.Services.OrderAPI.Services.IServices;
using Mango.Services.OrderAPI.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;

namespace Mango.Services.OrderAPI.Controllers
{
	[Route("api/order")]
	[ApiController]
	public class OrderAPIController : ControllerBase
	{
		private ResponseDto _responseDto;
		private IMapper _mapper;
		private readonly AppDbContext _db;
		private readonly IProductServices _productServices;

		public OrderAPIController(AppDbContext db, IMapper mapper, IProductServices productService)
		{
			_db = db;
			_mapper = mapper;
			_productServices = productService;
			this._responseDto = new ResponseDto();
		}

		[Authorize]
		[HttpPost("createorder")]
		public async Task<ResponseDto> CreateOrder([FromBody] CartDto cartDto)
		{
			try
			{
				OrderHeaderDto orderHeaderDto = _mapper.Map<OrderHeaderDto>(cartDto.CartHeader);
				orderHeaderDto.OrderTime = DateTime.Now;
				orderHeaderDto.Status = SD.Status_Pending;
				orderHeaderDto.OrderDetails = _mapper.Map<IEnumerable<OrderDetailDto>>(cartDto.CartDetails);

				OrderHeader orderCreated = _db.OrderHeaders.Add(_mapper.Map<OrderHeader>(orderHeaderDto)).Entity;

				await _db.SaveChangesAsync();

				orderHeaderDto.OrderHeaderId = orderCreated.OrderHeaderId;
				_responseDto.Result = orderHeaderDto;

			}
			catch (Exception ex)
			{
				_responseDto.isSuccess = false;
				_responseDto.Message = ex.Message;
			}
			return _responseDto;
		}

		[Authorize]
		[HttpPost("CreateStripeSession")]
		public async Task<ResponseDto> CreateStripeSession([FromBody] StripeRequestDto stripeRequestDto)
		{
			try
			{
				var options = new SessionCreateOptions
				{
					SuccessUrl = stripeRequestDto.ApprovedUrl,
					CancelUrl = stripeRequestDto.CancelUrl,
					LineItems = new List<SessionLineItemOptions>(),
					Mode = "payment",
				};

				var discountsObj = new List<SessionDiscountOptions>
				{
					new SessionDiscountOptions
					{
						Coupon = stripeRequestDto.OrderHeader.CouponCode
					}
				};

				foreach (var item in stripeRequestDto.OrderHeader.OrderDetails)
				{
					var sessionLineItem = new SessionLineItemOptions
					{
						PriceData = new SessionLineItemPriceDataOptions
						{
							UnitAmount = (long)(item.Price * 100), // Stripe expects the amount in cents
							Currency = "usd",
							ProductData = new SessionLineItemPriceDataProductDataOptions
							{
								Name = item?.ProductName
							},
						},
						Quantity = item.Count,
					};
					options.LineItems.Add(sessionLineItem);
				}

				if(stripeRequestDto.OrderHeader.Discount > 0)
				{
					options.Discounts = discountsObj;
				}
				var service = new SessionService();
				Session session = service.Create(options);
				stripeRequestDto.StripeSessionUrl = session.Url;
				OrderHeader orderHeader = _db.OrderHeaders.First(u => u.OrderHeaderId == stripeRequestDto.OrderHeader.OrderHeaderId);
				orderHeader.StripSessionId = session.Id;
				_db.SaveChanges();
				_responseDto.Result = stripeRequestDto;
			}
			catch (Exception ex)
			{
				_responseDto.isSuccess = false;
				_responseDto.Message = ex.Message;
			}
			return _responseDto;
		}
	}
}
