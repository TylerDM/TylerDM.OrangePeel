namespace TylerDM.OrangePeel;

public static class IServiceCollectionExt
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

			return services.addAll(assembly);
		}
	}
	#endregion

	#region private methods
	private static List<ServiceDescriptor> addAll(this IServiceCollection services, Assembly assembly)
	{
		var list = new List<ServiceDescriptor>();
		foreach (var implementationType in assembly.getConcreteClasses())
		{
			var directAttribute = implementationType.getAttribute();
			if (directAttribute is not null)
			{
				var lifetime = directAttribute.ServiceLifetime;
				list.Add(services.add(lifetime, implementationType));
				foreach (var interfaceType in directAttribute.AdditionalInterfaces)
					list.Add(services.add(lifetime, implementationType, interfaceType));
			}

			foreach (var interfaceType in implementationType.getPotentialInterfaceTypes())
			{
				var interfaceAttribute = interfaceType.getAttribute();
				if (interfaceAttribute is null) continue;

				var lifetime = directAttribute?.ServiceLifetime ?? interfaceAttribute.ServiceLifetime;
				list.Add(services.add(lifetime, implementationType, interfaceType));
				foreach (var additionalInterface in interfaceAttribute.AdditionalInterfaces)
					list.Add(services.add(lifetime, implementationType, additionalInterface));
			}
		}
		return list;
	}

	private static IEnumerable<Type> getConcreteClasses(this Assembly assembly) =>
		assembly.GetTypes().Where(x => x.IsClass && x.IsAbstract == false);

	private static DependencyInjectableAttribute? getAttribute(this Type type) =>
		type.GetCustomAttribute<DependencyInjectableAttribute>(false);

	private static IEnumerable<Type> getPotentialInterfaceTypes(this Type type) =>
		type.SelectFollow(x => x.BaseType).Concat(type.GetInterfaces());

	private static ServiceDescriptor add(this IServiceCollection services, ServiceLifetime lifetime, Type implementationType, Type? interfaceType = null)
	{
		interfaceType ??= implementationType;

		var descriptor = new ServiceDescriptor(interfaceType, implementationType, lifetime);
		services.Add(descriptor);
		return descriptor;
	}
	#endregion
}