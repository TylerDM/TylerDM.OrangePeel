using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace TylerDM.OrangePeel
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class DependencyInjectableAttribute : Attribute
	{
		public ServiceLifetime ServiceLifetime { get; }
		public IReadOnlyCollection<Type> InterfaceTypes { get; }

		public DependencyInjectableAttribute(ServiceLifetime serviceLifetime)
		{
			ServiceLifetime = serviceLifetime;
			InterfaceTypes = Array.Empty<Type>();
		}

		public DependencyInjectableAttribute(ServiceLifetime serviceLifetime, params Type[] interfaceTypes)
		{
			ServiceLifetime = serviceLifetime;
			InterfaceTypes = interfaceTypes ?? throw new ArgumentNullException(nameof(interfaceTypes));
		}
	}
}