using Microsoft.AspNetCore.Mvc;
using Catalog.Services;
using Catalog.Models;

namespace Catalog.Controllers
{
    [ApiController]
    [Route("api/catalog")]
    public class CatalogController : ControllerBase
    {
        private readonly ICatalogService _catalogService;

        public CatalogController(ICatalogService catalogService)
        {
            _catalogService = catalogService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Book>>> GetAll() =>
            Ok(await _catalogService.GetBooksAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetById(string id)
        {
            var book = await _catalogService.GetBookByIdAsync(id);
            return book is null ? NotFound() : Ok(book);
        }

        [HttpPost]
        public async Task<ActionResult> Update(string id, Book book)
        {
            if (id != book.Id)
            {
                return BadRequest();
            }

            await _catalogService.UpdateAsync(book);
            return NoContent(); 
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _catalogService.DeleteAsync(id);
            return NoContent();
                
        }
    }
}
