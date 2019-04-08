using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace TylerDM.OrangePeel
{
  public static class IServiceCollectionExt
  {
    private static bool alreadyRan = false;

    public static void AddOrangePeeledServices(this IServiceCollection services)
    {
      if (services == null) throw new ArgumentNullException(nameof(services));

      if (alreadyRan) return;
      alreadyRan = true;

      foreach (var type in getAllTypesInDomain())
      {
        var attributes = type.GetCustomAttributes<DependencyInjectableAttribute>();
        if (!attributes.Any()) continue;

        var attribute = attributes.First();

        services.Add(attribute.ServiceLifetime, type);
        foreach (var interfaceType in attribute.InterfaceTypes)
          services.Add(attribute.ServiceLifetime, type, interfaceType);
      }
    }

    public static void Add(this IServiceCollection services, ServiceLifetime serviceLifetime, Type service, Type interfaceType = null)
    {
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
