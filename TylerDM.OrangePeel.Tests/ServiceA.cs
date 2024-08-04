namespace TylerDM.OrangePeel.Tests;

[DependencyInjectable(ServiceLifetime.Transient, typeof(IInterfaceA))]
public class ServiceA : IInterfaceA
{
}
