namespace TylerDM.OrangePeel;

public class Tests(
	IServiceCollection _services,
	GenericBaseClassD<bool>? _genericBaseClassD,
	IInterfaceC? _interfaceC,
	BaseClassC? _baseClassC
)
{
	[Fact]
	public void RegisterGenericBaseClass()
	{
		Assert.NotNull(_genericBaseClassD);
		Assert.Equal(typeof(ImplementationClassD), _genericBaseClassD.GetType());
	}

	[Fact]
	public void LifetimeAccurary()
	{
		void assertLifeTime(ServiceLifetime lifetime, Type serviceType) =>
			Assert.Equal(lifetime, getDescriptor(serviceType).Lifetime);

		assertLifeTime(ServiceLifetime.Scoped, typeof(ServiceB));
		assertLifeTime(ServiceLifetime.Transient, typeof(IInterfaceA));
	}

	[Fact]
	public void AdditionalTypes()
	{
		var descriptorA = getDescriptor(typeof(IInterfaceA));
		Assert.Equal(typeof(ServiceA), descriptorA.ImplementationType);
	}

	[Fact]
	public void PreventDoubleRegistration()
	{
		var serviceCollection = new ServiceCollection();
		Assert.Empty(serviceCollection.AddOrangePeeledServices());
	}

	[Fact]
	public void InheritedInterfaceRegistrations()
	{
		Assert.NotNull(_interfaceC);
		Assert.Equal(typeof(ImplementationClassC), _interfaceC.GetType());
	}

	[Fact]
	public void InheritedClassRegistrations()
	{
		Assert.NotNull(_baseClassC);
		Assert.Equal(typeof(ImplementationClassC), _baseClassC.GetType());
	}

	private ServiceDescriptor getDescriptor(Type serviceType) =>
		_services.First(x => x.ServiceType == serviceType);
}
