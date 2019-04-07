# Orange Peel
Attribute-centric Dependency Injection helpers.

If you're like me you've stopped at some point, looked at your thousands of lines of DI registrations, and thought that maybe it wasn't the best use of time.  Or you may have looked at them and thought, hey, these classes know what their lifetimes should be, why is that defined in a separate file?

This project aims to, in the smallest way possible, solve these two issues. To start, define the DI configuration on your classes.
```C#
[DependencyInjected(ServiceLifetime.Transient)]
class Car
{
  //...
}
```

Next just call the extension method while building your `ServiceCollection`.
```C#
serviceCollection.AddOrangePeeledServices();
```

If you need to register the class with various interfaces, just reference the types in your constructor.
```C#
[DependencyInjected(ServiceLifetime.Singleton, typeof(IEngine), typeof(IMachine))]
public class BigEngine : IEngine, IMachine
{
  //...
}
```
`params` is used here so you can add as many as you want easily.
