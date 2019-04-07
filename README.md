# OrangePeel
Attribute-centric Dependency Injection helpers.

If you're like me youve stopped at some point, looked at your thousands of lijes of DI registrations, and thought that maybe it wasn't the best use of time.  Or you may have looked at them and thought, hey, these classes know what their lifetimes should be, why is that defined in a separate file?

This project aims to, in the snallest way possible, solve these two issues. To start, define the DI configuration on your classes.
```C#
[DependencyInjected(ServiceLifetime.Transient)]
class Car
{
  //...
}
```

Next just call th
 extension method while building your `ServiceCollection`.
