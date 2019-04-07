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
          foreach (var interfaceType in attribute.InterfaceTypes)
            services.add(attribute.ServiceLifetime, interfaceType, type);
        }
      }
    }

    private static void add(this IServiceCollection services, ServiceLifetime serviceLifetime, Type type)
    {
      switch (serviceLifetime)
      {
        case ServiceLifetime.Singleton:
          services.AddSingleton(type);
          break;
        case ServiceLifetime.Scoped:
          services.AddScoped(type);
          break;
        case ServiceLifetime.Transient:
          services.AddTransient(type);
          break;
      }
    }

    private static void add(this IServiceCollection services, ServiceLifetime serviceLifetime, Type interfaceType, Type concreteType)
    {
      switch (serviceLifetime)
      {
        case ServiceLifetime.Singleton:
          services.AddSingleton(interfaceType, concreteType);
          break;
        case ServiceLifetime.Scoped:
          services.AddScoped(interfaceType, concreteType);
          break;
        case ServiceLifetime.Transient:
          services.AddTransient(interfaceType, concreteType);
          break;
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
