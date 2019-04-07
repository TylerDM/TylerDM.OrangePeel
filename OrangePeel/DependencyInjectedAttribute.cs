using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace TylerDM.OrangePeel
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
  public class DependencyInjectedAttribute : Attribute
  {
    public ServiceLifetime ServiceLifetime { get; }
    public IReadOnlyCollection<Type> InterfaceTypes { get; }

    public DependencyInjectedAttribute(ServiceLifetime serviceLifetime)
    {
      ServiceLifetime = serviceLifetime;
      InterfaceTypes = new Type[0];
    }

    public DependencyInjectedAttribute(ServiceLifetime serviceLifetime, params Type[] interfaceTypes)
    {
      ServiceLifetime = serviceLifetime;
      InterfaceTypes = interfaceTypes ?? throw new ArgumentNullException(nameof(interfaceTypes));
    }
  }
}
