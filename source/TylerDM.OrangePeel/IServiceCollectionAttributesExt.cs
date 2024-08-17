namespace TylerDM.OrangePeel;

public static class IServiceCollectionAttributesExt
{
	#region fields
	private static readonly ConcurrentBag<Assembly> _orangePeeledAssemblies = [];
	#endregion

	#region methods
	public static IReadOnlyCollection<ServiceDescriptor> AddOrangePeeledServices(this IServiceCollection services)
	{
		ArgumentNullException.ThrowIfNull(services);

		//This must execute here and CANNOT be moved into a different method as then the calling assembly would be Orange Peel itself.
		var assembly = Assembly.GetCallingAssembly();
		return services.AddOrangePeeledServices(assembly);
	}

	public static IReadOnlyCollection<ServiceDescriptor> AddOrangePeeledServices(this IServiceCollection services, Assembly assembly)
	{
		ArgumentNullException.ThrowIfNull(services);
		ArgumentNullException.ThrowIfNull(assembly);

		//Make sure we orange peel any given assembly only once.
		lock (_orangePeeledAssemblies)
		{
			if (_orangePeeledAssemblies.Contains(assembly)) return [];
			_orangePeeledAssemblies.Add(assembly);

			var list = new List<ServiceDescriptor>();
			list.AddRange(services.registerDirect(assembly));
			list.AddRange(services.registerIndirect(assembly));
			return list;
		}
	}
	#endregion

	#region private methods
	private static IEnumerable<ServiceDescriptor> registerDirect(this IServiceCollection services, Assembly assembly)
	{
		foreach (var (implementationType, attribute) in getDirectRegistrations(assembly))
		{
			var lifetime = attribute.ServiceLifetime;
			yield return services.register(lifetime, implementationType);
			foreach (var additionalInterface in attribute.AdditionalInterfaces)
				yield return services.register(lifetime, implementationType, additionalInterface);
		}
	}

	private static IEnumerable<Registration> getDirectRegistrations(Assembly assembly) =>
		from implementationType in assembly.GetDeveloperTypes()
		where implementationType.IsClass && implementationType.IsAbstract == false
		let attribute = implementationType.getAttribute()
		where attribute is not null
		select new Registration(implementationType, attribute);

	private static IEnumerable<ServiceDescriptor> registerIndirect(this IServiceCollection services, Assembly assembly)
	{
		foreach (var (interfaceType, interfaceAttribute) in getIndirectRegistrations(assembly))
			foreach (var implementation in getImplementationsOf(interfaceType))
			{
				var lifetime = implementation.Attribute?.ServiceLifetime ?? interfaceAttribute.ServiceLifetime;
				foreach (var closedInterfaces in implementation.ClosedInterfaceTypes)
					yield return services.register(lifetime, implementation.Type, closedInterfaces);
			}
	}

	private static IEnumerable<Implementation> getImplementationsOf(Type interfaceType) =>
		//AppDomainExt.GetImplementingTypes() allows us to register from other assemblies.
		from implementationType in AppDomainExt.GetImplementingTypes(interfaceType)
		where implementationType.IsClass == true && implementationType.IsAbstract == false
		let attribute = implementationType.getAttribute()
		select new Implementation(implementationType, attribute, interfaceType);

	private static IEnumerable<Registration> getIndirectRegistrations(Assembly assembly) =>
		from interfaceType in assembly.GetDeveloperTypes()
		where interfaceType.IsInterface || interfaceType.IsAbstract
		let attribute = interfaceType.getAttribute()
		where attribute is not null
		select new Registration(interfaceType, attribute);

	private static DiAttribute? getAttribute(this Type type) =>
		type.GetCustomAttribute<DiAttribute>(false);

	private static ServiceDescriptor register(this IServiceCollection services, ServiceLifetime lifetime, Type implementationType, Type? interfaceType = null)
	{
		interfaceType ??= implementationType;

		var descriptor = new ServiceDescriptor(interfaceType, implementationType, lifetime);
		services.Add(descriptor);
		return descriptor;
	}
	#endregion
}