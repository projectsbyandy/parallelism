using Ardalis.GuardClauses;
using Microsoft.Extensions.DependencyInjection;

namespace ParallelismExample.Tests.Management;

public class TestDependenciesManager : IDisposable
{
    private static readonly SemaphoreSlim Semaphore = new(1);
    private bool _disposedValue;
    private const int SemaphoreMaxWait = 1000;

    private static readonly Dictionary<string, TestDependenciesManager> TestDependenciesManagers = new();
    private static readonly Dictionary<string, ServiceProvider> ServiceProviders = new();

    public static void SetUp(ServiceProvider serviceProvider, string testId)
    {
        Semaphore.Wait(SemaphoreMaxWait);
        try
        {
            Console.WriteLine($"Trying to TestDependenciesManager-Setup with TestId: {testId}");

            var testDependenciesManager = new TestDependenciesManager(serviceProvider, testId);
            TestDependenciesManagers.TryAdd(testId, testDependenciesManager);
            Console.WriteLine("Number of test managers: " + TestDependenciesManagers.Count);
            Console.WriteLine("Number of service providers: " + ServiceProviders.Count);
        }
        finally
        {
            Semaphore.Release();
        }
    }

    public static void TearDown(string testId)
    {
        Semaphore.Wait(SemaphoreMaxWait);
        try
        {
            Console.WriteLine($"Trying to TestDependenciesManager-Teardown with TestId: {testId}");
            ServiceProviders.TryGetValue(testId, out var serviceProvider);
            
            Console.WriteLine($"About to Dispose of ServiceProvider: {serviceProvider?.GetHashCode()}");
            ServiceProviders.Remove(testId);
            serviceProvider?.Dispose();
            Console.WriteLine($"Disposed of ServiceProvider: {serviceProvider?.GetHashCode()}");

            TestDependenciesManagers.Remove(testId);
        }
        finally
        {
            Semaphore.Release();
        }
    }

    public static IServiceScope CreateScope(string testId)
    {
        Semaphore.Wait(SemaphoreMaxWait);
        try
        {
            Console.WriteLine($"Trying to TestDependenciesManager-CreateScope with TestId: {testId}");
            
            TestDependenciesManagers.TryGetValue(testId, out var testDependenciesManager);
            Guard.Against.Null(testDependenciesManager);
            
            ServiceProviders.TryGetValue(testId, out var serviceProvider);
            Guard.Against.Null(serviceProvider);
            
            var scope = serviceProvider.CreateScope();
            Console.WriteLine($"Scope hash: {scope.GetHashCode()}");
            
            return scope;
        }
        finally
        {
            Semaphore.Release();
        }
    }

    private TestDependenciesManager(ServiceProvider serviceProvider, string testId)
    {
        ServiceProviders.TryAdd(testId, serviceProvider);
    }

    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                Semaphore.Dispose();
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
}