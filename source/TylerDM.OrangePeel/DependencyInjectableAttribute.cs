namespace TylerDM.OrangePeel;

/// <summary>
/// When used on a class, it registers the class with the DI container.  When used on an an interface (including abstract/base classes), implementations of that interface are registered as the interface.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false)]
public sealed class DependencyInjectableAttribute : Attribute
{
	public ServiceLifetime ServiceLifetime { get; }
	/// <summary>
	/// Additional types to register as.
	/// </summary>
	public IReadOnlyCollection<Type> AdditionalInterfaces { get; }

	public DependencyInjectableAttribute(ServiceLifetime serviceLifetime) : this(serviceLifetime, [])
	{
	}

	public DependencyInjectableAttribute(ServiceLifetime serviceLifetime, params Type[] additionalInterfaces)
	{
		if (Enum.IsDefined(serviceLifetime) == false)
			throw new ArgumentOutOfRangeException(nameof(serviceLifetime));

		AdditionalInterfaces = additionalInterfaces ?? throw new ArgumentNullException(nameof(additionalInterfaces));
		ServiceLifetime = serviceLifetime;
	}
}