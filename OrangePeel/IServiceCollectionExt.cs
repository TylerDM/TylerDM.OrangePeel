using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TylerDM.OrangePeel
{
	public static class IServiceCollectionExt
	{
		private static readonly ConcurrentBag<string> _orangePeeledAssemblies = new ConcurrentBag<string>();

		public static AddServicesResult AddOrangePeeledServices(this IServiceCollection services)
		{
			if (services == null) throw new ArgumentNullException(nameof(services));

			//This must execute here and CANNOT be moved into a different method as then the calling assembly would be Orange Peel itself.
			var callingAssembly = Assembly.GetCallingAssembly();

			//Make sure we orangle peel any given assembly only once.
			var callingAssemblyName = callingAssembly.FullName ?? throw new Exception("Assembly name not found.");
			lock (_orangePeeledAssemblies)
			{
				if (_orangePeeledAssemblies.Contains(callingAssemblyName)) return AddServicesResult.Empty;
				_orangePeeledAssemblies.Add(callingAssemblyName);
			}

			return services.addAllTypesFromAssembly(callingAssembly);
		}

		private static AddServicesResult addAllTypesFromAssembly(this IServiceCollection services, Assembly assembly)
		{
			var addedServices = 0;
			var addedInterfaces = 0;
			foreach (var type in assembly.GetTypes())
			{
				var attributes = type.GetCustomAttributes<DependencyInjectableAttribute>();
				if (!attributes.Any()) continue;

				var attribute = attributes.First();
				var serviceLifetime = attribute.ServiceLifetime;

				if (type.IsAbstract) throw new Exception($"Cannot register abstract class \"{type.FullName}\".");

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

		private static void add(this IServiceCollection services, ServiceLifetime serviceLifetime, Type service, IEnumerable<Type> interfaceTypes)
		{
			if (services == null) throw new ArgumentNullException(nameof(services));
			if (service == null) throw new ArgumentNullException(nameof(service));
			if (interfaceTypes == null) throw new ArgumentNullException(nameof(interfaceTypes));

			foreach (var interfaceType in interfaceTypes)
				services.add(serviceLifetime, service, interfaceType);
		}

		private static void add(this IServiceCollection services, ServiceLifetime serviceLifetime, Type service)
		{
			if (services == null) throw new ArgumentNullException(nameof(services));
			if (service == null) throw new ArgumentNullException(nameof(service));

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
			if (services == null) throw new ArgumentNullException(nameof(services));
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
	}
}