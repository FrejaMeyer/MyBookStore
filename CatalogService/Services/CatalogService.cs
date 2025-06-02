using Catalog.Data;
using Catalog.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace Catalog.Services
{
    public interface ICatalogService
    {
        public Task<List<Book>> GetBooksAsync();
        public Task<Book?> GetBookByIdAsync(string id);
        public Task AddAsync(Book book);
        public Task UpdateAsync(Book book);
        public Task DeleteAsync(string id);

        public Task UpdateCachedInventoryAsync(string productId, int quantityAvailable);

    }
    public class CatalogService : ICatalogService
    {
        private readonly CatalogDbContext _dbContext;
        private readonly ILogger<CatalogService> _logger;
        private readonly ConcurrentDictionary<string, int> _inventoryCache = new();


        public CatalogService(CatalogDbContext dbContext, ILogger<CatalogService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        public async Task AddAsync(Book book)
        {
            _dbContext.Books.Add(book);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Book added:{Id} {Title}",book.Id, book.Title);
        }

        public Task UpdateCachedInventoryAsync(string productId, int quantityAvailable)
        {
            _inventoryCache[productId] = quantityAvailable;
            return Task.CompletedTask;
        }

        // Evt. en metode til at hente det senere
        public int? GetCachedInventory(string productId)
        {
            return _inventoryCache.TryGetValue(productId, out var qty) ? qty : (int?)null;
        }

        public async Task DeleteAsync(string id)
        {
            var book = await _dbContext.Books.FindAsync(id);
            if (book == null)
            {
                _dbContext.Books.Remove(book);
                await _dbContext.SaveChangesAsync();
                _logger.LogWarning("Book not found:{Id}", id);
            }
        }

        public async Task<Book?> GetBookByIdAsync(string id) => await _dbContext.Books.FindAsync(id);

        public async Task<List<Book>> GetBooksAsync() => await _dbContext.Books.ToListAsync();

        public async Task UpdateAsync(Book book)
        {
            _dbContext.Books.Update(book);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Book updated:{Id} {Title}",book.Id, book.Title);
        }
    }
}
