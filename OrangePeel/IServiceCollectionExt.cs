using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TylerDM.OrangePeel
{
	public static class IServiceCollectionExt
	{
		public static AddServicesResult AddOrangePeeledServices(this IServiceCollection services)
		{
			if (services == null) throw new ArgumentNullException(nameof(services));

			var addedServices = 0;
			var addedInterfaces = 0;

			//This must execute here and CANNOT be moved into getTypesFromCallingAssembly() as then the calling assembly would be itself.
			var callingAssembly = Assembly.GetCallingAssembly();
			foreach (var type in getTypesFromCallingAssembly(callingAssembly))
			{
				var attributes = type.GetCustomAttributes<DependencyInjectableAttribute>();
				if (!attributes.Any()) continue;

				var attribute = attributes.First();
				var serviceLifetime = attribute.ServiceLifetime;

				if (type.IsAbstract) throw new Exception($"Cannot register abstract class \"{type.FullName}\".");

				services.Add(serviceLifetime, type);
				addedServices++;

				if (attribute.InterfaceTypes.Any())
				{
					var interfaceTypes = attribute.InterfaceTypes;
					addedInterfaces += interfaceTypes.Count;
					services.Add(serviceLifetime, type, interfaceTypes);
				}
			}

			return new AddServicesResult(addedServices, addedInterfaces);
		}

		public static void Add(this IServiceCollection services, ServiceLifetime serviceLifetime, Type service, IEnumerable<Type> interfaceTypes)
		{
			if (services == null) throw new ArgumentNullException(nameof(services));
			if (service == null) throw new ArgumentNullException(nameof(service));
			if (interfaceTypes == null) throw new ArgumentNullException(nameof(interfaceTypes));

			foreach (var interfaceType in interfaceTypes)
				services.Add(serviceLifetime, service, interfaceType);
		}

		public static void Add(this IServiceCollection services, ServiceLifetime serviceLifetime, Type service)
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

		public static void Add(this IServiceCollection services, ServiceLifetime serviceLifetime, Type service, Type interfaceType)
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

		private static IEnumerable<Type> getTypesFromCallingAssembly(Assembly callingAssembly)
		{
			var types = callingAssembly.GetTypes();
			foreach (var type in types)
				yield return type;
		}
	}
}