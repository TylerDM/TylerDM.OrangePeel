using Microsoft.Extensions.DependencyInjection;
using System;
using TylerDM.OrangePeel;

namespace OrangePeelTest
{
	[DependencyInjectable(ServiceLifetime.Singleton, typeof(IEngine))]
	public class BigEngine : IEngine
	{
		public void Start()
		{
			Console.WriteLine("VROOM!");
		}
	}
}
