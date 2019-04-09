using Microsoft.Extensions.DependencyInjection;
using System;
using TylerDM.OrangePeel;

namespace OrangePeelTest
{
	class Program
	{
		static void Main(string[] args)
		{
			var serviceProvider = getServiceProvider();
			var car = serviceProvider.GetRequiredService<Car>();
			car.Start();
			Console.ReadKey();
		}

		private static IServiceProvider getServiceProvider()
		{
			var serviceCollection = new ServiceCollection();

			serviceCollection.AddOrangePeeledServices();

			return serviceCollection.BuildServiceProvider();
		}
	}
}
