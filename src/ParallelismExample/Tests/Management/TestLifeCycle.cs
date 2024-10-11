using System.Collections.Concurrent;
using Ardalis.GuardClauses;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using ParallelismExample.Repositories;
using ParallelismExample.Repositories.Internal;
using ParallelismExample.Services;
using ParallelismExample.Services.Internal;
using Serilog;

namespace ParallelismExample.Tests.Management;

public abstract class TestLifeCycle : IDisposable
{
    private bool _disposedValue;
    private readonly ConcurrentDictionary<string, IServiceScope> _scopes = new();
    
    protected IBatchService Publisher => GetService<IBatchService>(TestContext.CurrentContext.Test.ID);
    protected ILogger Logger => GetService<ILogger>(TestContext.CurrentContext.Test.ID);
    
    [SetUp]
    public void SetupScope()
    {
        Console.WriteLine($"TestId: {TestContext.CurrentContext.Test.ID}");
        
        var services = new ServiceCollection();
        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IBatchService, Publisher>();
        services.AddScoped<ILogger>(_ => new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .CreateLogger());

        TestDependenciesManager.SetUp(services.BuildServiceProvider(), TestContext.CurrentContext.Test.ID);
        _scopes.TryAdd(TestContext.CurrentContext.Test.ID, TestDependenciesManager.CreateScope(TestContext.CurrentContext.Test.ID));
        Console.WriteLine("Completed TestLifeCycle Nunit-Setup with TestId: " + TestContext.CurrentContext.Test.ID);
    }

    [TearDown]
    public void TearDownScope()
    {
        Logger.Information("Trying to NUnit-TearDown with TestId: {@TestId}", TestContext.CurrentContext.Test.ID);
        TestDependenciesManager.TearDown(TestContext.CurrentContext.Test.ID);
        _scopes.TryGetValue(TestContext.CurrentContext.Test.ID, out var scope);
        Guard.Against.Null(scope);
        _scopes.TryRemove(TestContext.CurrentContext.Test.ID, out _);
        scope?.Dispose();
    }
    
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _scopes.TryGetValue(TestContext.CurrentContext.Test.ID, out var scope);
                scope?.Dispose();
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    
    private T GetService<T>(string id)
    { 
        _scopes.TryGetValue(id, out var scope);
        Guard.Against.Null(scope);
        Console.WriteLine("Trying to Get object from scope: " + scope.GetHashCode() + " TestId: " + id);

        return Guard.Against.Null(scope.ServiceProvider.GetService<T>(), nameof(T),
            "Unable to retrieve from service provider");
    }
}