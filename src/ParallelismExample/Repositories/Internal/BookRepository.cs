using ParallelismExample.Models;

namespace ParallelismExample.Repositories.Internal;

internal class BookRepository : IBookRepository
{
    public async Task<IList<Book>> GetAllAsync()
    { 
        await Task.Delay(1000);
        return new List<Book>()
        {
            new() { Title = "The Dark Knight", Publisher = "DC" },
            new()
            {
                Title = "Batman Begins", Publisher = "DC"
            }
        };
    }
}