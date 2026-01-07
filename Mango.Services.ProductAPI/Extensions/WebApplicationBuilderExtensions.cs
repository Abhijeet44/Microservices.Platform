using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Runtime.CompilerServices;
using System.Text;

namespace Mango.Services.CouponAPI.Extensions
{
	public static class WebApplicationBuilderExtensions
	{
		public static WebApplicationBuilder AddAppAuthentication(this WebApplicationBuilder builder)
		{
			builder.Services.AddSwaggerGen(option =>
			{
				option.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, securityScheme: new OpenApiSecurityScheme
				{
					Name = "Authorization",
					Description = "Enter the Bearer string as following : `Bearer Generated-JWT-Token`",
					In = ParameterLocation.Header,
					Type = SecuritySchemeType.ApiKey,
					Scheme = "Bearer"
				});
				option.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Type = ReferenceType.SecurityScheme,
					Id = JwtBearerDefaults.AuthenticationScheme
				}
			},
			new string[] {}
		}
	});
			});

			var settingSection = builder.Configuration.GetSection("ApiSetting");

			var secret = settingSection.GetValue<string>("Secret");
			var Issuer = settingSection.GetValue<string>("Issuer");
			var Audience = settingSection.GetValue<string>("Audience");

			// Fix: Convert secret to byte[] for SymmetricSecurityKey
			var key = Encoding.ASCII.GetBytes(secret);

			builder.Services.AddAuthentication(x =>
			{
				x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(x =>
			{
				x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidIssuer = Issuer,
					ValidAudience = Audience,
					IssuerSigningKey = new SymmetricSecurityKey(key)
				};
			});

			return builder;
		}
	}
}
