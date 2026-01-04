using AutoMapper;
using Mango.Services.CouponAPI.Data;
using Mango.Services.CouponAPI.Model;
using Mango.Services.CouponAPI.Model.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace Mango.Services.CouponAPI.Controllers
{
	[Route("api/CouponAPI")]
	[ApiController]
	[Authorize]
	public class CouponAPIController : ControllerBase
	{
		private readonly AppDbContext _db;
		private readonly ResponseDto _response;
		private IMapper _mapper;

		public CouponAPIController(AppDbContext db, IMapper mapper)
		{
			_db = db;
			_response = new ResponseDto();
			_mapper = mapper;
		}

		[HttpGet]
		public ResponseDto Get()
		{
			try
			{
				IEnumerable<Coupon> objList = _db.Coupons.ToList();
				_response.Result = _mapper.Map<IEnumerable<CouponDto>>(objList);
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
		public ResponseDto Get(int id)
		{
			try
			{
				Coupon obj = _db.Coupons.First(u => u.CouponId == id);
				_response.Result = _mapper.Map<CouponDto>(obj);
			}
			catch (Exception ex)
			{
				_response.isSuccess = false;
				_response.Message = ex.Message;
			}
			return _response;
		}

		[HttpGet]
		[Route("GetByCoupon/{code}")]
		public ResponseDto GetByCode(string code)
		{
			try
			{
				Coupon obj = _db.Coupons.FirstOrDefault(u => u.CouponCode.ToLower() == code.ToLower());
				_response.Result = _mapper.Map<CouponDto>(obj);
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
		public ResponseDto Post([FromBody] CouponDto couponDto)
		{
			try
			{
				Coupon obj = _mapper.Map<Coupon>(couponDto);
				_db.Coupons.Add(obj);
				_db.SaveChanges();
				_response.Result = _mapper.Map<CouponDto>(obj);
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
		public ResponseDto Put([FromBody] CouponDto couponDto)
		{
			try
			{
				Coupon obj = _mapper.Map<Coupon>(couponDto);
				_db.Coupons.Update(obj);
				_db.SaveChanges();
				_response.Result = _mapper.Map<CouponDto>(obj);
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
		public ResponseDto Delete(int id)
		{
			try
			{
				Coupon obj = _db.Coupons.First(u => u.CouponId == id);
				_db.Coupons.Remove(obj);
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
