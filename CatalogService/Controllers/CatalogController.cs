using Catalog.Models;
using Catalog.Services;
using Microsoft.AspNetCore.Mvc;
using Dapr.Client;
using Shared.Dto;

namespace Catalog.Controllers
{
    [ApiController]
    [Route("api/catalog")]
    public class CatalogController : ControllerBase
    {
        private readonly ICatalogService _catalogService;
        private readonly DaprClient _daprClient; 
        private readonly ILogger<CatalogController> _logger;

        public CatalogController(ICatalogService catalogService, DaprClient daprClient, ILogger<CatalogController> logger)
        {
            _catalogService = catalogService;
            _daprClient = daprClient;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<Book>>> GetAll() =>
            Ok(await _catalogService.GetBooksAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetById(string id)
        {
            var item = await _catalogService.GetBookByIdAsync(id); // din interne metode

            if (item == null)
                return NotFound();

            var dto = new Book
            {
                Id = item.Id,
                Title = item.Title,
                Author = item.Author,
                Price = item.Price,
                Genre = item.Genre,
                Description = item.Description,
                ImageUrl = item.ImageUrl,
            };

            // Hent lager fra InventoryService
            try
            {
                var inventoryItem = await _daprClient.InvokeMethodAsync<InventoryItemDto>(
                  HttpMethod.Get,
                 "inventory",
                  $"inventory/{id}");

                dto.StockQuantity = inventoryItem?.QuantityAvailable;
            }
            catch
            {
                // Lagerdata ikke tilgængeligt – vis ikke noget
                dto.StockQuantity = null;
            }

            return Ok(dto);


            //var book = await _catalogService.GetBookByIdAsync(id);
            //return book is null ? NotFound() : Ok(book);
        }

        [HttpPost("{id}")]
        public async Task<ActionResult> Update(string id, [FromBody] Book book)
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
