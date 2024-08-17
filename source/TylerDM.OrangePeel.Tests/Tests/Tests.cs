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

	[Fact]
	public static void NoMissedRegistrations()
	{
		var descriptors = _serviceCollection.Where(x => x.ImplementationType == typeof(ImplementationClassC));
		if (descriptors.Count() != 3)
			throw new Exception($"{nameof(ImplementationClassC)} has the wrong number of registrations.");
	}

	[Fact]
	public static void RegisterGenericBaseClass()
	{
		var service = _serviceProvider.GetRequiredService<GenericBaseClassD<bool>>() ??
			throw new Exception($"No class registered for {nameof(GenericBaseClassD<bool>)}.");
		if (service is not ImplementationClassD)
			throw new Exception($"Incorrect class registered for {nameof(GenericBaseClassD<bool>)}. Expected {nameof(ImplementationClassD)}.");
	}

	[Fact]
	public static void LifetimeAccurary()
	{
		var descriptorB = _serviceCollection.First(x => x.ServiceType == typeof(ServiceB));
		if (descriptorB.Lifetime is not ServiceLifetime.Scoped)
			throw new Exception($"{nameof(ServiceB)} was not registered with the correct lifetime.");

		var descriptorA = _serviceCollection.First(x => x.ServiceType == typeof(IInterfaceA));
		if (descriptorA.Lifetime is not ServiceLifetime.Transient)
			throw new Exception($"{nameof(ServiceA)} through {nameof(IInterfaceA)} has incorrect lifetime.");
	}

	[Fact]
	public static void AdditionalTypes()
	{
		var descriptorA = _serviceCollection.First(x => x.ServiceType == typeof(IInterfaceA));
		if (descriptorA.ImplementationType != typeof(ServiceA))
			throw new Exception($"{nameof(ServiceA)} was not registered as {nameof(IInterfaceA)}");
	}

	[Fact]
	public static void Materialization()
	{
		var interfaceA = _serviceProvider.GetRequiredService<IInterfaceA>();
		if (interfaceA is not ServiceA)
			throw new Exception($"{nameof(ServiceProvider)} did not return {nameof(ServiceA)} for {nameof(IInterfaceA)}.");

		var serviceB = _serviceProvider.GetRequiredService<ServiceB>();
	}

	[Fact]
	public static void PreventDoubleRegistration()
	{
		var descriptors = _serviceCollection.AddOrangePeeledServices();
		if (descriptors.Count > 0)
			throw new Exception($"Assembly was Orange Peeled twice.");
	}

	[Fact]
	public static void InheritedInterfaceRegistrations()
	{
		var interfaceC = _serviceProvider.GetRequiredService<IInterfaceC>() ??
			throw new Exception($"No implementation was found for {nameof(IInterfaceC)}.  Expected {nameof(ImplementationClassC)}.");
		if (interfaceC is not ImplementationClassC)
			throw new Exception($"{nameof(ServiceProvider)} did not return {nameof(ImplementationClassC)} for {nameof(IInterfaceC)}.");
	}

	[Fact]
	public static void InheritedClassRegistrations()
	{
		var baseClassC = _serviceProvider.GetRequiredService<BaseClassC>() ??
			throw new Exception($"No implementation was found for {nameof(BaseClassC)}.  Expected {nameof(ImplementationClassC)}.");
		if (baseClassC is not ImplementationClassC)
			throw new Exception($"{nameof(ServiceProvider)} did not return {nameof(BaseClassC)} for {nameof(ImplementationClassC)}.");
	}
}
