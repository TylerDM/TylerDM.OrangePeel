namespace TylerDM.OrangePeel.Tests;

public static class Tests
{
	private static readonly ServiceCollection _serviceCollection;
	private static readonly ServiceProvider _serviceProvider;

	static Tests()
	{
		_serviceCollection = new ServiceCollection();
		_serviceCollection.AddOrangePeeledServices();
		_serviceProvider = _serviceCollection.BuildServiceProvider();
	}

	[Test]
	public static void LifetimeAccurary()
	{
		var descriptorB = _serviceCollection.First(x => x.ServiceType == typeof(ServiceB));
		if (descriptorB.Lifetime is not ServiceLifetime.Scoped)
			throw new Exception($"{nameof(ServiceB)} was not registered with the correct lifetime.");

		var descriptorA = _serviceCollection.First(x => x.ServiceType == typeof(IInterfaceA));
		if (descriptorA.Lifetime is not ServiceLifetime.Transient)
			throw new Exception($"{nameof(ServiceA)} through {nameof(IInterfaceA)} has incorrect lifetime.");
	}

	[Test]
	public static void AdditionalTypes()
	{
		var descriptorA = _serviceCollection.First(x => x.ServiceType == typeof(IInterfaceA));
		if (descriptorA.ImplementationType != typeof(ServiceA))
			throw new Exception($"{nameof(ServiceA)} was not registered as {nameof(IInterfaceA)}");
	}

	[Test]
	public static void Materialization()
	{
		var interfaceA = _serviceProvider.GetRequiredService<IInterfaceA>();
		if (interfaceA is not ServiceA)
			throw new Exception($"{nameof(ServiceProvider)} did not return {nameof(ServiceA)} for {nameof(IInterfaceA)}.");

		var serviceB = _serviceProvider.GetRequiredService<ServiceB>();
	}

	[Test]
	public static void PreventDoubleRegistration()
	{
		var descriptors = _serviceCollection.AddOrangePeeledServices();
		if (descriptors.Count > 0)
			throw new Exception($"Assembly was Orange Peeled twice.");
	}
}
