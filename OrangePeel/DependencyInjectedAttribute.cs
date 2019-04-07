using System;
using System.Collections.Generic;

namespace TylerDM.OrangePeel
{
	public enum ServiceLifetime
	{
		Singleton,
		Scoped,
		Transient
	}

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class DependencyInjectedAttribute : Attribute
	{
		public ServiceLifetime ServiceLifetime { get; }
		public IReadOnlyCollection<Type> InterfaceTypes { get; }

		public DependencyInjectedAttribute(ServiceLifetime serviceLifetime)
		{
			ServiceLifetime = serviceLifetime;
		}

		public DependencyInjectedAttribute(ServiceLifetime serviceLifetime, params Type[] interfaceTypes)
		{
			ServiceLifetime = serviceLifetime;
			InterfaceTypes = interfaceTypes ?? throw new ArgumentNullException(nameof(interfaceTypes));
		}
	}
}
