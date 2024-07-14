using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestServer.Core;
using TestServer.Handlers;
using Xunit;

namespace TestsForTestServer
{
    public class OrderDetailsHandlerTests
    {
        private readonly DbContextOptions<AppDbContext> _dbContextOptions;
        private readonly AppDbContext _context;

        public OrderDetailsHandlerTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                                .UseInMemoryDatabase(databaseName: "TestDatabase")
                                .Options;
            _context = new AppDbContext(_dbContextOptions);
        }

        private async Task SeedDatabase()
        {
            _context.OrderTable.RemoveRange(_context.OrderTable);
            await _context.SaveChangesAsync(); 

            var orderDetails = new List<OrderTable>
            {
                new OrderTable { OrderId = 1, OrderName = "Order 1", Date = DateOnly.FromDateTime(DateTime.Now), Count = 1, Cost = 100 },
                new OrderTable { OrderId = 2, OrderName = "Order 2", Date = DateOnly.FromDateTime(DateTime.Now), Count = 2, Cost = 200 }
            };

            await _context.OrderTable.AddRangeAsync(orderDetails);
            await _context.SaveChangesAsync();
        }


        [Fact]
        public async Task AddToDb_AddsOrderDetailsToDatabase()
        {
            var orderDetails = new OrderDetails(DateOnly.FromDateTime(DateTime.Now), "Order 1", 1, 100);

            OrderDetailsHandler.AddToDb(orderDetails, _context);

            var addedOrder = await _context.OrderTable.FindAsync(orderDetails.OrderId);
            Assert.NotNull(addedOrder);
            Assert.Equal(orderDetails.OrderName, addedOrder.OrderName);
        }

        [Fact]
        public async Task ChangeOrder_UpdatesOrderDetailsInDatabase()
        {
            await SeedDatabase();

            var updatedName = "Updated Order";
            await OrderDetailsHandler.ChangeOrder(1, DateOnly.FromDateTime(DateTime.Now), updatedName, 2, 200, _context);

            var updatedOrder = await _context.OrderTable.FindAsync(1);
            Assert.NotNull(updatedOrder);
            Assert.Equal(updatedName, updatedOrder.OrderName);
        }

        [Fact]
        public async Task DeleteOrder_RemovesOrderDetailsFromDatabase()
        {
            await SeedDatabase();

            await OrderDetailsHandler.DeleteOrder(1, _context);

            var deletedOrder = await _context.OrderTable.FindAsync(1);
            Assert.Null(deletedOrder);
        }
    }
}
