using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestServer.Controllers;
using TestServer.Core;
using Xunit;

namespace TestsForTestServer
{
    public class OrderDetailsControllerTests : IAsyncLifetime
    {
        private readonly DbContextOptions<AppDbContext> _dbContextOptions;
        private readonly AppDbContext _context;

        public OrderDetailsControllerTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                                .UseInMemoryDatabase(databaseName: "TestDatabase")
                                .Options;
            _context = new AppDbContext(_dbContextOptions);
        }

        public async Task InitializeAsync()
        {
            await SeedDatabase();
        }

        public Task DisposeAsync()
        {
            _context.Dispose();
            return Task.CompletedTask;
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
        public async Task Get_ReturnsOkResult_WithListOfOrderDetails()
        {
            await SeedDatabase();

            var controller = new OrderDetailsController(_context);
            var result = await controller.Get();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var orders = Assert.IsType<List<OrderTable>>(okResult.Value);
            Assert.Equal(2, orders.Count);
        }

        [Fact]
        public async Task GetById_ReturnsOkResult_WithOrderDetail()
        {
            await SeedDatabase();

            var controller = new OrderDetailsController(_context);
            var result = await controller.GetById(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var order = Assert.IsType<OrderTable>(okResult.Value);
            Assert.Equal(1, order.OrderId);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenOrderDoesNotExist()
        {
            await SeedDatabase();

            var controller = new OrderDetailsController(_context);
            var result = await controller.GetById(999);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Post_ReturnsOkResult_WithCreatedOrder()
        {
            await SeedDatabase();

            var controller = new OrderDetailsController(_context);
            var result = await controller.Post(DateOnly.FromDateTime(DateTime.Now), "Order 3", 3, 300);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var order = Assert.IsType<OrderDetails>(okResult.Value);
            Assert.Equal("Order 3", order.OrderName);
        }

        [Fact]
        public async Task Put_ReturnsOkResult_WhenOrderIsUpdated()
        {
            await SeedDatabase();

            var controller = new OrderDetailsController(_context);
            var result = await controller.Put(1, DateOnly.FromDateTime(DateTime.Now), "Updated Order", 10, 1000);

            Assert.IsType<OkResult>(result);

            var updatedOrder = await _context.OrderTable.FindAsync(1);
            Assert.Equal("Updated Order", updatedOrder.OrderName);
            Assert.Equal(10, updatedOrder.Count);
            Assert.Equal(1000, updatedOrder.Cost);
        }

        [Fact]
        public async Task Put_ReturnsNotFound_WhenOrderDoesNotExist()
        {
            await SeedDatabase();

            var controller = new OrderDetailsController(_context);
            var result = await controller.Put(999, DateOnly.FromDateTime(DateTime.Now), "Updated Order", 10, 1000);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsOkResult()
        {
            await SeedDatabase();

            var controller = new OrderDetailsController(_context);
            var result = await controller.Delete(1);

            Assert.IsType<OkResult>(result);

            var deletedOrder = await _context.OrderTable.FindAsync(1);
            Assert.Null(deletedOrder);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenOrderDoesNotExist()
        {
            await SeedDatabase();

            var controller = new OrderDetailsController(_context);
            var result = await controller.Delete(999);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
