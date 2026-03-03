using Mango.Services.RewardAPI.Data;
using Mango.Services.RewardAPI.Message;
using Mango.Services.RewardAPI.Models;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Mango.Services.RewardAPI.Services
{
	public class RewardService : IRewardService
	{
		private DbContextOptions<AppDbContext> options;

		public RewardService(DbContextOptions<AppDbContext> options)
		{
			this.options = options;
		}

		private async Task UpdateRewards(RewardMessage rewardMessage)
		{
			try
			{
				Reward reward = new()
				{
					OrderId = rewardMessage.OrderId,
					RewardsActivity = rewardMessage.RewardsActivity,
					UserId = rewardMessage.UserId,
					RewardDate = DateTime.Now
				};

				await using var _db = new AppDbContext(options);
				await _db.Rewards.AddAsync(reward);
				await _db.SaveChangesAsync();
			}
			catch (Exception ex)
			{
			}
		}

		Task IRewardService.UpdateRewards(RewardMessage rewardMessage)
		{
			return UpdateRewards(rewardMessage);
		}
	}
}
