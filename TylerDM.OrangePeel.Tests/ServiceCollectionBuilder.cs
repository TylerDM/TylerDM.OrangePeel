namespace TylerDM.OrangePeel.Tests;

public static class ServiceCollectionBuilder
{
	public static IServiceProvider CreateServiceProvider(Action<IServiceCollection>? action)
	{
		var serviceCollection = new ServiceCollection();
		serviceCollection.AddOrangePeeledServices();
		if (action is not null)
			action(serviceCollection);
		return serviceCollection.BuildServiceProvider();
	}
}
