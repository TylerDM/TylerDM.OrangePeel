using Microsoft.Extensions.DependencyInjection;
using System;
using TylerDM.OrangePeel;

namespace OrangePeelTest
{
  [DependencyInjected(ServiceLifetime.Singleton, typeof(IEngine))]
  public class BigEngine : IEngine
  {
    public void Start()
    {
      Console.WriteLine("VROOM!");
    }
  }
}
