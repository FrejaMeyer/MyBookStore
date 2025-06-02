using Catalog.Models;
using Catalog.Services;
using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
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
        [Topic("pubsub", "catalog.inventory.updated")]
        [HttpPost("inventory-updated")]
        public async Task<IActionResult> HandleInventoryUpdated([FromBody] InventoryResponse response)
        {
            // Gem i f.eks. Redis, MemoryCache, Database – eller push til frontend via SignalR
            await _catalogService.UpdateCachedInventoryAsync(response.ProductId, response.QuantityAvailable);

            return Ok();
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

            await _daprClient.PublishEventAsync("pubsub", "inventory.request", new InventoryRequest()
            {
                ProductId = id,
                ReplyTo = "catalog.inventory.response"  
            });
            // Hent lager fra InventoryService

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
