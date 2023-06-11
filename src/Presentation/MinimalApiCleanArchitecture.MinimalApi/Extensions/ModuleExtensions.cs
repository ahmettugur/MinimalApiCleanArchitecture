using MinimalApiCleanArchitecture.MinimalApi.Abstractions;

namespace MinimalApiCleanArchitecture.MinimalApi.Extensions;
public static class ModuleExtensions
{
    private static List<IModule> _modules = new();

    public static WebApplicationBuilder RegisterModules(this WebApplicationBuilder builder)
    {
        _modules = DiscoverModule();
        foreach (var module in _modules)
        {
            module.RegisterModule(builder);

        }
        return builder;
    }

    public static WebApplication MapEndpoints(this WebApplication app)
    {
        foreach (var module in _modules)
        {
            module.MapEndpoints(app);
        }
        return app;
    }
    private static List<IModule> DiscoverModule()
    {
        return typeof(IModule).Assembly
            .GetTypes()
            .Where(m => m.IsClass && m.IsAssignableTo(typeof(IModule)))
            .Select(Activator.CreateInstance)
            .Cast<IModule>()
            .ToList();
    }
}
