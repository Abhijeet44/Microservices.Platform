using Mango.Services.RewardAPI.Messaging;

namespace Mango.Services.RewardAPI.Extension
{
	public static class ApplicationBuilderExtension
	{
		private static IAzureServiceBusConsumer serviceBusConsumer {  get; set; }

		public static IApplicationBuilder UseAzureServiceBusConsumer(this IApplicationBuilder app)
		{
			serviceBusConsumer = app.ApplicationServices.GetService<IAzureServiceBusConsumer>();
			var hostApplicationLife = app.ApplicationServices.GetService<IHostApplicationLifetime>();

			hostApplicationLife.ApplicationStarted.Register(OnStart);
			hostApplicationLife.ApplicationStarted.Register(OnStop);

			return app;
		}

		private static void OnStop()
		{
			throw new NotImplementedException();
		}

		private static void OnStart()
		{
			throw new NotImplementedException();
		}
	}
}
