namespace TylerDM.OrangePeel;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class DependencyInjectableAttribute : Attribute
{
	public ServiceLifetime ServiceLifetime { get; }
	public IReadOnlyCollection<Type> ServiceTypes { get; }

	public DependencyInjectableAttribute(ServiceLifetime serviceLifetime) : this(serviceLifetime, [])
	{
	}

	public DependencyInjectableAttribute(ServiceLifetime serviceLifetime, params Type[] interfaceTypes)
	{
		if (Enum.IsDefined(serviceLifetime) == false)
			throw new ArgumentOutOfRangeException(nameof(serviceLifetime));

		ServiceTypes = interfaceTypes ?? throw new ArgumentNullException(nameof(interfaceTypes));
		ServiceLifetime = serviceLifetime;
	}
}