namespace ParallelismExample.Services;

public interface IBatchService
{
    public Task PublishAsync(string title);
}