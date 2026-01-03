using Mango.Services.AuthAPI.Model.Dto;
using Mango.Services.AuthAPI.Models.Dto;
using Mango.Services.AuthAPI.Services.Iservice;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Mango.Services.AuthAPI.Controllers
{
	[Route("api/auth")]
	[ApiController]
	public class AuthAPIController : ControllerBase
	{
		private readonly IAuthService _authService;
		protected readonly ResponseDto _response;
		public AuthAPIController(IAuthService authService)
		{
			_authService = authService;
			_response = new ResponseDto();
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegistrationRequestDto models)
		{
			var errrorMessage = await _authService.RegisterAsync(models);
			if(!string.IsNullOrEmpty(errrorMessage))
			{
				_response.IsSuccess = false;
				_response.Message = errrorMessage;
				return BadRequest(_response);

			}
			return Ok(_response);
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
		{
			var loginResponse = await _authService.LoginAsync(model);
			if (loginResponse.User == null)
			{
				_response.IsSuccess=false;
				_response.Message = "Username or password is Incorrect";
				return BadRequest(_response);
			}
			_response.Result = loginResponse;
			return Ok(_response);
		}

		[HttpPost("AssignRole")]
		public async Task<IActionResult> AssignRole([FromBody] RegistrationRequestDto model)
		{
			var assignRoleSuccessful = await _authService.AssignRole(model.Email, model.Role);
			if (!assignRoleSuccessful)
			{
				_response.IsSuccess = false;
				_response.Message = "Error Encountered";
				return BadRequest(_response);
			}
			return Ok(_response);
		}
	}
}
