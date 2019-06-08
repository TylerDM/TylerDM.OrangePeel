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
          services.add(attribute.ServiceLifetime, type);
          services.add(attribute.ServiceLifetime, type, attribute.InterfaceTypes);
        }
      }
    }

    private static void add(this IServiceCollection services, ServiceLifetime serviceLifetime, Type concreteType, IEnumerable<Type> interfaceTypes)
    {
      foreach (var interfaceType in interfaceTypes)
        services.add(serviceLifetime, concreteType, interfaceType);
    }

    private static void add(this IServiceCollection services, ServiceLifetime serviceLifetime, Type concreteType, Type interfaceType = null)
    {
      interfaceType = interfaceType ?? concreteType;

      switch (serviceLifetime)
      {
        case ServiceLifetime.Singleton:
          services.AddSingleton(interfaceType, x => x.GetService(concreteType));
          break;
        case ServiceLifetime.Scoped:
          services.AddScoped(interfaceType, x => x.GetService(concreteType));
          break;
        case ServiceLifetime.Transient:
          services.AddTransient(interfaceType, x => x.GetService(concreteType));
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof(serviceLifetime));
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
