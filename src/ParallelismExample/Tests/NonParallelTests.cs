using System.Diagnostics;
using NUnit.Framework;
using ParallelismExample.Tests.Management;

namespace ParallelismExample.Tests;

internal class NonParallelTests : TestLifeCycle
{
    private readonly Stopwatch _stopwatch = new();
    
    [OneTimeSetUp]
    public void StartTimer() => _stopwatch.Start();

    [Test]
    public async Task NonParallelTest1() => await ExecuteQueryTimesAsync(3);
    
    [Test]
    public async Task NonParallelTest2() => await ExecuteQueryTimesAsync(3);
    
    [Test]
    public async Task NonParallelTest3() => await ExecuteQueryTimesAsync(3);
    
    [OneTimeTearDown]
    public void StopTimer()
    {
        _stopwatch.Stop();
        Console.WriteLine($"Non-Parallel tests took: {_stopwatch.ElapsedMilliseconds.ToString()}ms to run");
    }

    private async Task ExecuteQueryTimesAsync(int count)
    {
        Logger.Information("Publisher hashcode : {@HashCode}",Publisher.GetHashCode());

        for (var i = count - 1; i >= 0; i--)
        {
            await Publisher.PublishAsync("The Dark Knight");
        }
    }
}