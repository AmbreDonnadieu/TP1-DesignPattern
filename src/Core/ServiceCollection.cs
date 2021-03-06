using System;
using System.Collections.Generic;
using System.Reflection;


// C'est le conteneur d'injection de d�pendance
//tu enregistres les types � cr�er et tu appelles l'injection de d�pendance quand tu veux cr�er un objet
// On lui donne une liste d'objet � cr�er avec des indications sur le type de constructeurs d'objet � utiliser
internal interface IServiceProvider
{
    object GetService(Type serviceType);

    bool HasService(Type serviceType);

    bool TryGetService(Type serviceType, out object serviceInstance);
}

internal static class IServiceProviderExtensions
{
    public static TService GetService<TService>(this IServiceProvider provider) where TService: class
    {
        return provider.GetService(typeof(TService)) as TService;
    }

    public static object InstantiateService(this IServiceProvider serviceProvider, Type serviceType)
    {
        ConstructorInfo reqconstructor = null;

        foreach(ConstructorInfo constructor in serviceType.GetConstructors())
        {
            bool allArgumentAreServices = true;
            foreach(Type argument in constructor.GetGenericArguments())
            {
                if(!serviceProvider.HasService(argument))
                {
                    allArgumentAreServices = false;
                    break;
                }
            }

            if(!allArgumentAreServices)
            {
                continue;
            }

            reqconstructor = constructor;
            break;
        }

        if(reqconstructor != null)
        {
            Type[] argTypes = reqconstructor.GetGenericArguments();
            object[] args = new object[argTypes.Length];

            for(int i = 0; i < argTypes.Length; i++)
            {
                args[i] = serviceProvider.GetService(argTypes[i]);
            }

            return reqconstructor.Invoke(args);
        }

        return Activator.CreateInstance(serviceType);
    }
}

internal interface IServiceRegistry
{
    void AddService(Type serviceType, Type serviceImplType);

    void AddService(Type serviceType, Func<IServiceProvider, object> factory);
}

internal static class IServiceRegistryExtensions
{
    public static void AddService<TService>(this IServiceRegistry serviceRegistry, Func<IServiceProvider, TService> factory) where TService: class
    {
        serviceRegistry.AddService(typeof(TService), factory);
    }

    public static void AddService<TService, TServiceImpl>(this IServiceRegistry serviceRegistry) where TService: class where TServiceImpl: class, TService
    {
        serviceRegistry.AddService(typeof(TService), typeof(TServiceImpl));
    }
}

internal class ServiceCollection: IServiceProvider, IServiceRegistry
{
    private Dictionary<Type, Func<IServiceProvider, object>> RegisteredServices = new Dictionary<Type, Func<IServiceProvider, object>>();
    private Dictionary<Type, object> InstantiatedServices = new Dictionary<Type, object>();

    public ServiceCollection()
    {
        RegisterAssembly(typeof(JobSystem.JobSystem).Assembly);
    }

    public void RegisterAssembly(Assembly assembly)
    {
        foreach(Type type in assembly.GetTypes()) {
            if (type.IsClass && type.GetCustomAttributes(typeof(ServiceAttribute), true).Length > 0) {
                RegisteredServices.Add(type, (IServiceProvider serviceProvider) => {
                    return serviceProvider.InstantiateService(type);
                });
            }
        }
    }

    public void AddService(Type serviceType, Type serviceImplType)
    {
        AddService(serviceType, (IServiceProvider serviceProvider) => {
            return serviceProvider.InstantiateService(serviceImplType);
        });
    }

    public void AddService(Type serviceType, Func<IServiceProvider, object> factory)
    {
        RegisteredServices.Add(serviceType, factory);
    }

    public object GetService(Type serviceType)
    {
        object service;
        TryGetService(serviceType, out service);

        if(service == null)
        {
            throw new Exception("No service of type " + serviceType.FullName + " found.");
        }

        return service;
    }

    public bool HasService(Type serviceType)
    {
        return RegisteredServices.ContainsKey(serviceType);
    }

    public bool TryGetService(Type serviceType, out object serviceInstance)
    {
        if(InstantiatedServices.TryGetValue(serviceType, out serviceInstance))
        {
            return true;
        } 
        else if(RegisteredServices.TryGetValue(serviceType, out Func<IServiceProvider, object> serviceFactory))
        {
            serviceInstance = serviceFactory(this);
            InstantiatedServices.Add(serviceType, serviceInstance);
            return true;
        }

        return false;
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class ServiceAttribute : Attribute  
{
    public ServiceAttribute()  
    { 
    }  
}
