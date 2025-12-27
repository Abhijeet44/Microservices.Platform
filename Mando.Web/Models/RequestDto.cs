using Mando.Web.Utility;
using static Mando.Web.Utility.SD;

namespace Mando.Web.Models
{
	public class RequestDto
	{
		public ApiType APIType { get; set; } = ApiType.GET;

		public string URL { get; set; }

		public object? Data { get; set; }

		public string AccessToken { get; set; } 
	}
}
