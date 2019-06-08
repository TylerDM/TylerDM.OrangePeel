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

			foreach (var type in getAllTypesInDomain())
			{
				var attributes = type.GetCustomAttributes<DependencyInjectableAttribute>();
				if (!attributes.Any()) continue;

				var attribute = attributes.First();
				var serviceLifetime = attribute.ServiceLifetime;

				if (attribute.InterfaceTypes.Any())
				{
					var interfaceTypes = attribute.InterfaceTypes;
					addedInterfaces += interfaceTypes.Count;
					services.Add(serviceLifetime, type, interfaceTypes);
				}
				else
				{
					if (type.IsAbstract) throw new Exception($"Cannot register abstract class \"{type.FullName}\".  Did you forget to include an interface?");

					services.Add(serviceLifetime, type);
					addedServices++;
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

		public static void Add(this IServiceCollection services, ServiceLifetime serviceLifetime, Type service, Type interfaceType = null)
		{
			if (services == null) throw new ArgumentNullException(nameof(services));
			if (service == null) throw new ArgumentNullException(nameof(service));

			interfaceType = interfaceType ?? service;

			switch (serviceLifetime)
			{
				case ServiceLifetime.Singleton:
					services.AddSingleton(interfaceType, x => x.GetRequiredService(service));
					break;
				case ServiceLifetime.Scoped:
					services.AddScoped(interfaceType, x => x.GetRequiredService(service));
					break;
				case ServiceLifetime.Transient:
					services.AddTransient(interfaceType, x => x.GetRequiredService(service));
					break;

				default:
					throw new ArgumentOutOfRangeException(nameof(serviceLifetime));
			}
		}

		private static IEnumerable<Type> getAllTypesInDomain()
		{
			var assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (var assembly in assemblies)
			{
				var types = assembly.GetTypes();
				foreach (var type in types)
					yield return type;
			}
		}
	}
}
