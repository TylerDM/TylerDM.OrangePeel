# Orange Peel

Attribute-centric Dependency Injection helpers.

If you're like me you've stopped at some point, looked at your thousands of lines of DI registrations, and thought that maybe it wasn't the best use of time. Or you may have looked at them and thought, hey, these classes know what their lifetimes should be, why is that defined in a separate file?

This project aims to, in the smallest way possible, solve these two issues. To start, define the DI configuration on your classes.

    [DependencyInjectable(ServiceLifetime.Transient)]
    class Car
    {
        //...
    }

Next just call the extension method while building your `ServiceCollection`. This must be done by each assembly which uses Orange Peel.

    public static class MyLibraryStartup
    {
        public static void AddMyLibrary(this IServiceCollection services)
        {
            services.AddOrangePeeledServices();
        }
    }

If you need to register the class with various interfaces, just reference the types in your attribute's constructor.

    [DependencyInjectable(ServiceLifetime.Singleton, typeof(IEngine), typeof(IMachine))]
    class BigEngine : IEngine, IMachine
    {
        //...
    }

`params` is used here so you can add as many as you want easily.

You can also auto-register all implementations of an abstract/base class or interface by applying the attribute to the abstract/base class or interface. This will register implementations from all assemblies in the App Domain, not just the assembly Orange Peel is currently discovering registrations from.

[Nuget](https://www.nuget.org/packages/TylerDM.OrangePeel)