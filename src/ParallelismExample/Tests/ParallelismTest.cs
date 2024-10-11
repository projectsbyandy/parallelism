using System.Diagnostics;
using NUnit.Framework;
using ParallelismExample.Tests.Management;

namespace ParallelismExample.Tests;

[Parallelizable(ParallelScope.All)]
internal class ParallelismTest : TestLifeCycle
{
    private readonly Stopwatch _stopwatch = new();
    
    [OneTimeSetUp]
    public void StartTimer()
    {
        _stopwatch.Start();
    }

    [Test]
    public async Task ParallelTest1() => await ExecuteQueryTimesAsync(3);
    
    [Test]
    public async Task ParallelTest2() => await ExecuteQueryTimesAsync(3);
    
    [Test]
    public async Task ParallelTest3() => await ExecuteQueryTimesAsync(3);
    
    private async Task ExecuteQueryTimesAsync(int count)
    {
        Logger.Information("Publisher hashcode : {@HashCode}",Publisher.GetHashCode());

        for (var i = count - 1; i >= 0; i--)
        {
            await Publisher.PublishAsync("The Dark Knight");
        }
    }

    [OneTimeTearDown]
    public void StopTimer()
    {
        _stopwatch.Stop();
        Console.WriteLine($"Parallel tests took: {_stopwatch.ElapsedMilliseconds.ToString()}ms to run");
    }
}