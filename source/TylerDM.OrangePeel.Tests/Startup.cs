﻿namespace TylerDM.OrangePeel;

public class Startup
{
	public void ConfigureServices(IServiceCollection services)
	{
		services.AddOrangePeeledServices();
		services.AddSingleton(services);
	}
}
