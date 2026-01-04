using Mando.Web.Models;
using Mando.Web.Services.IService;
using Mango.Web.Utility;
using Mango.Web.Model;
using Newtonsoft.Json;
using System.Text;
using Mango.Web.Services.IService;

namespace Mando.Web.Services
{
	public class BaseService : IBaseService
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly ITokenProvider _tokenProvider;

		public BaseService(IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider)
		{
			_httpClientFactory = httpClientFactory;
			_tokenProvider = tokenProvider;
		}

		public async Task<ResponseDto?> SendAsync(RequestDto requestDto, bool withBearer = true)
		{
			try
			{
				HttpClient client = _httpClientFactory.CreateClient("MangoAPI");
				HttpRequestMessage message = new HttpRequestMessage();
				message.Headers.Add("Accept", "application/json");
				if (withBearer)
				{
					var token =  _tokenProvider.GetToken();
					message.Headers.Add("Authorization", $"Bearer {token}");
				}
				message.RequestUri = new Uri(requestDto.URL);
				if (requestDto.Data != null)
				{
					message.Content = new StringContent(JsonConvert.SerializeObject(requestDto.Data), Encoding.UTF8, "application/json");
				}

				HttpResponseMessage? apiResponse = null;

				switch (requestDto.APIType)
				{
					case SD.ApiType.POST:
						message.Method = HttpMethod.Post;
						break;
					case SD.ApiType.PUT:
						message.Method = HttpMethod.Put;
						break;
					case SD.ApiType.DELETE:
						message.Method = HttpMethod.Delete;
						break;
					default:
						message.Method = HttpMethod.Get;
						break;
				}

				apiResponse = await client.SendAsync(message);

				switch (apiResponse.StatusCode)
				{
					case System.Net.HttpStatusCode.NotFound:
						return new() { isSuccess = false, Message = "NOT FOUND" };
					case System.Net.HttpStatusCode.Forbidden:
						return new() { isSuccess = false, Message = "ACCESS DENIED" };
					case System.Net.HttpStatusCode.InternalServerError:
						return new() { isSuccess = false, Message = "INTERNAL SERVER ERROR" };
					case System.Net.HttpStatusCode.Unauthorized:
						return new() { isSuccess = false, Message = "UNAUTHORISED" };
					default:
						var apiContent = await apiResponse.Content.ReadAsStringAsync();
						var responseDto = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
						return responseDto;
				}
			}
			catch (Exception ex)
			{
				var dto = new ResponseDto
				{
					isSuccess = false,
					Message = ex.Message
				};
				return dto;
			}
		}
	}
}
