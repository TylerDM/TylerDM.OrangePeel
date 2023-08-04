namespace TylerDM.OrangePeel;

public static class IServiceCollectionExt
{
	#region fields
	private static readonly ConcurrentBag<string> _orangePeeledAssemblies = new();
	#endregion

	#region methods
	public static AddServicesResult AddOrangePeeledServices(this IServiceCollection services)
	{
		//This must execute here and CANNOT be moved into a different method as then the calling assembly would be Orange Peel itself.
		var assembly = Assembly.GetCallingAssembly();

		//Make sure we orange peel any given assembly only once.
		var callingAssemblyName = assembly.FullName ?? throw new Exception("Assembly name not found.");
		lock (_orangePeeledAssemblies)
		{
			if (_orangePeeledAssemblies.Contains(callingAssemblyName)) return new(0, 0);
			_orangePeeledAssemblies.Add(callingAssemblyName);
		}

		return services.addAllTypesFromAssembly(assembly);
	}
	#endregion

	#region private methods
	private static AddServicesResult addAllTypesFromAssembly(this IServiceCollection services, Assembly assembly)
	{
		var addedServices = 0;
		var addedInterfaces = 0;
		foreach (var type in getTypesSafely(assembly))
		{
			var attributes = type.GetCustomAttributes<DependencyInjectableAttribute>();
			if (!attributes.Any()) continue;

			if (type.IsAbstract) throw new Exception($"Cannot register abstract class \"{type.FullName}\".");

			var attribute = attributes.First();
			var serviceLifetime = attribute.ServiceLifetime;

			services.add(serviceLifetime, type);
			addedServices++;

			if (attribute.InterfaceTypes.Any())
			{
				var interfaceTypes = attribute.InterfaceTypes;
				addedInterfaces += interfaceTypes.Count;
				services.add(serviceLifetime, type, interfaceTypes);
			}
		}

		return new AddServicesResult(addedServices, addedInterfaces);
	}

	private static IEnumerable<Type> getTypesSafely(Assembly assembly)
	{
		//Less dependencies are loaded in Load() and LoadFrom().
		assembly = Assembly.Load(assembly.GetName());
		try
		{
			return assembly.GetTypes();
		}
		catch (ReflectionTypeLoadException exception)
		{
			return exception.Types.OfType<Type>();
		}
	}

	private static void add(this IServiceCollection services, ServiceLifetime serviceLifetime, Type service, IEnumerable<Type> interfaceTypes)
	{
		if (services is null) throw new ArgumentNullException(nameof(services));
		if (service is null) throw new ArgumentNullException(nameof(service));
		if (interfaceTypes is null) throw new ArgumentNullException(nameof(interfaceTypes));

		foreach (var interfaceType in interfaceTypes)
			services.add(serviceLifetime, service, interfaceType);
	}

	private static void add(this IServiceCollection services, ServiceLifetime serviceLifetime, Type service)
	{
		if (services is null) throw new ArgumentNullException(nameof(services));
		if (service is null) throw new ArgumentNullException(nameof(service));

		switch (serviceLifetime)
		{
			case ServiceLifetime.Singleton:
				services.AddSingleton(service);
				break;
			case ServiceLifetime.Scoped:
				services.AddScoped(service);
				break;
			case ServiceLifetime.Transient:
				services.AddTransient(service);
				break;

			default:
				throw new ArgumentOutOfRangeException(nameof(serviceLifetime));
		}
	}

	private static void add(this IServiceCollection services, ServiceLifetime serviceLifetime, Type service, Type? interfaceType)
	{
		if (services is null) throw new ArgumentNullException(nameof(services));
		interfaceType ??= service ?? throw new ArgumentNullException(nameof(service));

		switch (serviceLifetime)
		{
			case ServiceLifetime.Singleton:
				services.AddSingleton(interfaceType, x => x.GetRequiredService(service));
				break;
			case ServiceLifetime.Scoped:
				services.AddScoped(interfaceType, service);
				break;
			case ServiceLifetime.Transient:
				services.AddTransient(interfaceType, service);
				break;

			default:
				throw new ArgumentOutOfRangeException(nameof(serviceLifetime));
		}
	}
	#endregion
}