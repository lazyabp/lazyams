using Autofac;
using Microsoft.Extensions.DependencyModel;
using System.Reflection;
using System.Runtime.Loader;

namespace Lazy.Application;

public class AutofacModule : Autofac.Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder
            .RegisterType<AuthenticationService>()
            .As<IAuthenticationService>()
            .InstancePerDependency();

        var dependencyContext = DependencyContext.Default;
        if (dependencyContext == null)
        {
            throw new InvalidOperationException("DependencyContext.Default 为空，无法加载运行时库。");
        }

        var compilationLibrary = dependencyContext.RuntimeLibraries.Where(x => x.Name.StartsWith("Lazy."));
        List<Assembly> assemblyList = new List<Assembly>();
        foreach (var iLibrary in compilationLibrary)
        {
            try
            {
                assemblyList.Add(AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(iLibrary.Name)));
            }
            catch (Exception ex)
            {
                Console.WriteLine(iLibrary.Name + ex.Message);
            }
        }
        var assemblys = assemblyList.ToArray();
        builder.RegisterAssemblyTypes(assemblys).Where(x => x.IsAssignableTo<ITransientDependency>())
            .AsImplementedInterfaces()
            .InstancePerDependency()
            .PropertiesAutowired();

        builder.RegisterAssemblyTypes(assemblys).Where(x => x.IsAssignableTo<IScopedDependency>())
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope()
            .PropertiesAutowired(); 

        builder.RegisterAssemblyTypes(assemblys).Where(x => x.IsAssignableTo<ISingletonDependency>())
            .AsImplementedInterfaces()
            .SingleInstance()
            .PropertiesAutowired();


    }
}
