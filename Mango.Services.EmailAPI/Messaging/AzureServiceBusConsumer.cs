using Azure.Messaging.ServiceBus;
using Mango.Services.EmailAPI.Models.Dtos;
using Mango.Services.EmailAPI.Services;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Services.EmailAPI.Messaging
{
	public class AzureServiceBusConsumer : IAzureServiceBusConsumer
	{
		private readonly string serviceBusConnectionString;
		private readonly string emailCartQueue;
		private readonly string registerUserQueue;
		private readonly IConfiguration _configuration;
		private EmailService _emailService;

		private ServiceBusProcessor _emailCartProcessor;
		private ServiceBusProcessor _registerUserProcessor;

		public AzureServiceBusConsumer(IConfiguration configuration, EmailService emailService)
		{
			_configuration = configuration;
			_emailService = emailService;

			serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");

			emailCartQueue = _configuration.GetValue<string>("TopicAndQueueNames:EmailShopingCartQueue");
			registerUserQueue = _configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueueQueue");

			var client = new ServiceBusClient(serviceBusConnectionString);
			_emailCartProcessor = client.CreateProcessor(emailCartQueue);
			_registerUserProcessor = client.CreateProcessor(registerUserQueue);
		}

		public async Task Start()
		{
			_emailCartProcessor.ProcessMessageAsync += OnUserRegisterEmailRequestRecieved;
			_emailCartProcessor.ProcessErrorAsync += ErrorHandler;
			_emailCartProcessor.StartProcessingAsync();

			_registerUserProcessor.ProcessMessageAsync += OnEmailCartEmailRequestRecieved;
			_registerUserProcessor.ProcessErrorAsync += ErrorHandler;
			_registerUserProcessor.StartProcessingAsync();
		}

		public async Task Stop()
		{
			await _emailCartProcessor.StopProcessingAsync();
			await _emailCartProcessor.DisposeAsync();

			await _registerUserProcessor.StopProcessingAsync();
			await _registerUserProcessor.DisposeAsync();
		}

		private Task ErrorHandler(ProcessErrorEventArgs args)
		{
			Console.WriteLine(args.Exception.ToString());
			return Task.CompletedTask;
		}

		private async Task OnEmailCartEmailRequestRecieved(ProcessMessageEventArgs args)
		{
			// this is where we will recieve the message
			var message = args.Message;
			var body = Encoding.UTF8.GetString(message.Body);

			CartDto objMessage = JsonConvert.DeserializeObject<CartDto>(body);

			try
			{
				// try to log email
				await _emailService.EmailCartAndLog(objMessage);
				await args.CompleteMessageAsync(args.Message);
			}
			catch
			{
				throw;
			}
		}

		private async Task OnUserRegisterEmailRequestRecieved(ProcessMessageEventArgs args)
		{
			var message = args.Message;
			var body = Encoding.UTF8.GetString(message.Body);

			string email = JsonConvert.DeserializeObject<string>(body);

			try
			{
				// try to log email
				await _emailService.RegisterdEmailAndLog(email);
				await args.CompleteMessageAsync(args.Message);
			}
			catch
			{
				throw;
			}
		}
	}
}
