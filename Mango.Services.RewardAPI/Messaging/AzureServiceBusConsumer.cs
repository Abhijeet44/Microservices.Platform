using Azure.Messaging.ServiceBus;
using Mango.Services.RewardAPI.Message;
using Mango.Services.RewardAPI.Models;
using Mango.Services.RewardAPI.Services;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Services.RewardAPI.Messaging
{
	public class AzureServiceBusConsumer : IAzureServiceBusConsumer
	{
		private readonly string serviceBusConnectionString;
		private readonly string orderCreatedTopic;
		private readonly string orderCreatedRewardSubscription;
		private readonly IConfiguration _configuration;
		private IRewardService _rewardService;

		private ServiceBusProcessor _rewardProcessor;
		
		public AzureServiceBusConsumer(IConfiguration configuration, IRewardService rewardService)
		{
			_configuration = configuration;
			_rewardService = rewardService;

			serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");

			orderCreatedTopic = _configuration.GetValue<string>("TopicandQueueName:OrderCreatedTopic");
			orderCreatedRewardSubscription = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreated_Reward_subscription");

			var client = new ServiceBusClient(serviceBusConnectionString);
			_rewardProcessor = client.CreateProcessor(orderCreatedTopic, orderCreatedRewardSubscription);
		}

		public async Task Start()
		{
			_rewardProcessor.ProcessMessageAsync += OnNewOredrRequestRecieved;
			_rewardProcessor.ProcessErrorAsync += ErrorHandler;
			_rewardProcessor.StartProcessingAsync();

		}

		public async Task Stop()
		{
			await _rewardProcessor.StopProcessingAsync();
			await _rewardProcessor.DisposeAsync();
		}

		private Task ErrorHandler(ProcessErrorEventArgs args)
		{
			Console.WriteLine(args.Exception.ToString());
			return Task.CompletedTask;
		}

		private async Task OnNewOredrRequestRecieved(ProcessMessageEventArgs args)
		{
			// this is where we will recieve the message
			var message = args.Message;
			var body = Encoding.UTF8.GetString(message.Body);

			RewardMessage objMessage = JsonConvert.DeserializeObject<RewardMessage>(body);

			try
			{
				// try to log email
				await _rewardService.UpdateRewards(objMessage);
				await args.CompleteMessageAsync(args.Message);
			}
			catch
			{
				throw;
			}
		}
	}
}
