namespace Mango.Services.AuthAPI.Models.Dto
{
	public class JWtOptions
	{
		public string Secret { get; set; }
		public string Issuer { get; set; }
		public string Audience { get; set; }
	}
}
