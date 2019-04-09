using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Collections.Generic;
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

        services.Add(serviceLifetime, type);
        addedServices++;

        foreach (var interfaceType in attribute.InterfaceTypes)
        {
          services.Add(serviceLifetime, type, interfaceType);
          addedInterfaces++;
        }
      }

      return new AddServicesResult(addedServices, addedInterfaces);
    }

    public static void Add(this IServiceCollection services, ServiceLifetime serviceLifetime, Type service, Type interfaceType = null)
    {
      if (services == null) throw new ArgumentNullException(nameof(services));
      if (service == null) throw new ArgumentNullException(nameof(service));

      switch (serviceLifetime)
      {
        case ServiceLifetime.Singleton:
          services.AddSingleton(interfaceType ?? service, service);
          break;
        case ServiceLifetime.Scoped:
          services.AddScoped(interfaceType ?? service, service);
          break;
        case ServiceLifetime.Transient:
          services.AddTransient(interfaceType ?? service, service);
          break;
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
