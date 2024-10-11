using ParallelismExample.Models;

namespace ParallelismExample.Repositories;

public interface IBookRepository
{
    public Task<IList<Book>> GetAllAsync();
}