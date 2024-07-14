using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestServer.Core;
using TestServer.Handlers;

namespace TestServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderDetailsController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public OrderDetailsController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet(Name = "Get")]
        public async Task<IActionResult> Get()
        {
            var result = await _dbContext.OrderTable.ToListAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _dbContext.OrderTable.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        [HttpPost(Name = "Post")]
        public async Task<IActionResult> Post([FromQuery] DateOnly date, [FromQuery] string name, [FromQuery] int count, [FromQuery] int cost)
        {
            OrderDetails orderDetails = new OrderDetails(date, name, count, cost);
            try
            {
                if (orderDetails == null)
                {
                    return BadRequest();
                }

                OrderDetailsHandler.AddToDb(orderDetails, _dbContext);
                return Ok(orderDetails);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "An error occurred while processing the file: " + e.Message);
            }
        }

        [HttpPut(Name = "Put")]
        public async Task<IActionResult> Put([FromQuery] int id, [FromQuery] DateOnly? date,
                                             [FromQuery] string? name, [FromQuery] int? count, [FromQuery] int? cost)
        {
            if (!_dbContext.OrderTable.Any(x => x.OrderId == id))
            {
                return NotFound();
            }

            await OrderDetailsHandler.ChangeOrder(id, date, name, count, cost, _dbContext);
            return Ok();
        }

        [HttpDelete(Name = "Delete")]
        public async Task<IActionResult> Delete([FromQuery] int id)
        {
            if (!_dbContext.OrderTable.Any(x => x.OrderId == id))
            {
                return NotFound();
            }

            await OrderDetailsHandler.DeleteOrder(id, _dbContext);
            return Ok();
        }
    }
}
