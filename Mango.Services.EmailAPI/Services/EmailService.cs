using Mango.Services.EmailAPI.Data;
using Mango.Services.EmailAPI.Model;
using Mango.Services.EmailAPI.Models.Dtos;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Mango.Services.EmailAPI.Services
{
	public class EmailService : IEmailService
	{
		private DbContextOptions<AppDbContext> options;

		public EmailService(DbContextOptions<AppDbContext> options)
		{
			this.options = options;
		}

		public async Task EmailCartAndLog(CartDto cartDto)
		{
			StringBuilder message = new StringBuilder();
			message.AppendLine("<br>New Cart Created ");
			message.AppendLine("<br> Total :" + cartDto.CartHeader.CartTotal);
			message.AppendLine("<br>");
			message.AppendLine("<ul/>");
			foreach(var item in cartDto.CartDetails)
			{
				message.AppendLine("<li>");
		        message.Append(item.Product.Name + " X " + item.Count);
				message.Append("</li>");
			}
			message.AppendLine("<ul/>");

			await LogAndEmail(message.ToString(), cartDto.CartHeader.Email);

		}

		public async Task RegisterdEmailAndLog(string email)
		{
			string message = "User Registration successful <br/> Email" + email;
			await LogAndEmail(message, "Admin@gmil.com");
		}

		private async Task<bool> LogAndEmail(string message, string email)
		{
			try
			{
				EmailLogger emailLogger = new EmailLogger()
				{
					Email = email,
					EmailSent = DateTime.Now,
					Message = message
				};

				await using var _db = new AppDbContext(options);
				await _db.EmailLoggers.AddAsync(emailLogger);
				await _db.SaveChangesAsync();
				return true;
			}
			catch (Exception ex)
			{
				return false;
			}
		}
	}
}
