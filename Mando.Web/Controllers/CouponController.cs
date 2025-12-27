using Mando.Web.Services.IService;
using Mango.Web.Model;
using Mango.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
	public class CouponController : Controller
	{
		private readonly ICouponService _couponService;
		private readonly IBaseService _baseService;
		public CouponController(IBaseService baseService, ICouponService couponService) 
		{ 
			_couponService = couponService;
			_baseService = baseService;
		}

		public async Task<IActionResult> CouponIndex()
		{
			List<CouponDto>? list = new();

			ResponseDto? response = await _couponService.GetAllCouponsAsync();

			if (response != null && response.isSuccess)
			{
				list = JsonConvert.DeserializeObject<List<CouponDto>>(Convert.ToString(response.Result));
			}
			else
			{
				TempData["Error"] = response?.Message;
			}

				return View(list);
		}

		public async Task<IActionResult> CouponCreate()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> CouponCreate(CouponDto model)
		{
			if (ModelState.IsValid)
			{
				ResponseDto? response = await _couponService.CreateCouponAsync(model);
				if (response != null && response.isSuccess)
				{
					TempData["Success"] = "Coupon Created Successfully";
					return RedirectToAction(nameof(CouponIndex));
				}
				else
				{
					TempData["Error"] = response?.Message;
				}
			}

			return View(model);
		}

		public async Task<IActionResult> CouponDelete(int couponId)
		{
			ResponseDto? response = await _couponService.GetCouponsByIdAsync(couponId);
			if (response != null && response.isSuccess)
			{
				CouponDto? model = JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(response.Result));
				return View(model);
			}
			else
			{
				TempData["Error"] = response?.Message;
			}
			return NotFound();
		}

		[HttpPost]
		public async Task<IActionResult> CouponDelete(CouponDto couponDto)
		{
			ResponseDto? response = await _couponService.DeleteCouponAsync(couponDto.CouponId);
			if (response != null && response.isSuccess)
			{
				TempData["Success"] = "Coupon Deleted Successfully";
				return RedirectToAction(nameof(CouponIndex));
			}
			else
			{
				TempData["Error"] = response?.Message;
			}
			return View(couponDto);
		}
	}
}
