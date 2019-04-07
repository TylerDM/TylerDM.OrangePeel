using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace TylerDM.OrangePeel
{
	public static class IServiceCollectionExt
	{
		public static void AddOrangePeeledServices(this IServiceCollection services)
		{
			if (services == null) throw new ArgumentNullException(nameof(services));

			foreach (var type in getAllTypesInAssembly())
			{
				var attributes = type.GetCustomAttributes<DependencyInjectedAttribute>();
				if (!attributes.Any()) continue;

				foreach (var attribute in attributes)
				{
					switch (attribute.ServiceLifetime)
					{
						case ServiceLifetime.Singleton:
							services.AddSingleton(type);
							foreach(var interfaceType in attribute.InterfaceTypes)
								services.AddSingleton(interfaceType, type);
							break;
						case ServiceLifetime.Scoped:
							services.AddScoped(type);
							foreach (var interfaceType in attribute.InterfaceTypes)
								services.AddScoped(interfaceType, type);
							break;
						case ServiceLifetime.Transient:
							services.AddTransient(type);
							foreach (var interfaceType in attribute.InterfaceTypes)
								services.AddTransient(interfaceType, type);
							break;
					}
				}
			}
		}

		private static IEnumerable<Type> getAllTypesInAssembly()
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
