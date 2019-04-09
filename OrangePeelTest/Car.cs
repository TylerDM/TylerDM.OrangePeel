using Microsoft.Extensions.DependencyInjection;
using System;
using TylerDM.OrangePeel;

namespace OrangePeelTest
{
  [DependencyInjectable(ServiceLifetime.Singleton)]
  public class Car
  {
    private readonly IEngine _engine;

    public Car(IEngine engine)
    {
      _engine = engine ?? throw new ArgumentNullException(nameof(engine));
    }

    public void Start()
    {
      _engine.Start();
    }
  }
}
