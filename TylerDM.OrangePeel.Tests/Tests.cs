namespace TylerDM.OrangePeel.Tests;

public static class Tests
{
	private static readonly ServiceCollection _serviceCollection;

	static Tests()
	{
		_serviceCollection = new ServiceCollection();
		_serviceCollection.AddOrangePeeledServices();
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
}
