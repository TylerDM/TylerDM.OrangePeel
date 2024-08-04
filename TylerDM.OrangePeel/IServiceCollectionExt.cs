namespace TylerDM.OrangePeel;

public static class IServiceCollectionExt
{
	#region fields
	private static readonly List<Assembly> _orangePeeledAssemblies = [];
	#endregion

	#region methods
	public static IReadOnlyCollection<ServiceDescriptor> AddOrangePeeledServices(this IServiceCollection services)
	{
		ArgumentNullException.ThrowIfNull(services);

		//This must execute here and CANNOT be moved into a different method as then the calling assembly would be Orange Peel itself.
		var assembly = Assembly.GetCallingAssembly();

		//Make sure we orange peel any given assembly only once.
		lock (_orangePeeledAssemblies)
		{
			if (_orangePeeledAssemblies.Contains(assembly)) return [];
			_orangePeeledAssemblies.Add(assembly);
		}

		return services.addAll(assembly);
	}
	#endregion

	#region private methods
	private static List<ServiceDescriptor> addAll(this IServiceCollection services, Assembly assembly)
	{
		var list = new List<ServiceDescriptor>();
		foreach (var (serviceType, attribute) in getServiceTypes(assembly))
		{
			if (serviceType.IsAbstract)
				throw new Exception($"Cannot register abstract class {serviceType.FullName}.");

			list.AddRange(services.add(serviceType, attribute));
		}
		return list;
	}

	private static List<ServiceDescriptor> add(this IServiceCollection services, Type implementationType, DependencyInjectableAttribute attribute)
	{
		var list = new List<ServiceDescriptor>(1 + attribute.ServiceTypes.Count);
		var lifetime = attribute.ServiceLifetime;

		list.Add(services.add(lifetime, implementationType));
		foreach (var serviceType in attribute.ServiceTypes)
			list.Add(services.add(lifetime, implementationType, serviceType));

		return list;
	}

	private static IEnumerable<(Type ServiceType, DependencyInjectableAttribute Attribute)> getServiceTypes(Assembly assembly) =>
		from type in assembly.GetTypes()
		let attribute = type.GetCustomAttributes<DependencyInjectableAttribute>().FirstOrDefault()
		where attribute is not null
		select (type, attribute);

	private static ServiceDescriptor add(this IServiceCollection services, ServiceLifetime lifetime, Type implementationType, Type? serviceType = null)
	{
		serviceType ??= implementationType;

		var descriptor = new ServiceDescriptor(serviceType, implementationType, lifetime);
		services.Add(descriptor);
		return descriptor;
	}
	#endregion
}