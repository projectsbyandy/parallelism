using ParallelismExample.Repositories;
using Serilog;

namespace ParallelismExample.Services.Internal;

internal class Publisher(IBookRepository bookRepository, ILogger _logger) : IBatchService
{
    public async Task PublishAsync(string bookTitle)
    {
        var books = await bookRepository.GetAllAsync();
        var book = books.FirstOrDefault(book => book.Title.Equals(bookTitle, StringComparison.InvariantCulture));
        _logger.Information("Published book {@Book}", book);
    }
}