using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Moq.Protected;
using ConsoleResult;

namespace TestsForConsole
{
    public class OrderDetailsServiceTests
    {
        [Fact]
        public async Task GetOrderDetailsAsync_ReturnsListOfOrderDetails()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = JsonContent.Create(new List<OrderDetails>
                    {
                        new OrderDetails(DateOnly.Parse("2023-01-01"), "Test Order 1", 1, 100),
                        new OrderDetails(DateOnly.Parse("2023-01-02"), "Test Order 2", 2, 200)
                    })
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://localhost:7021/")
            };

            var service = new OrderDetailsService(httpClient);

            // Act
            var result = await service.GetOrderDetailsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Test Order 1", result[0].OrderName);
            Assert.Equal("Test Order 2", result[1].OrderName);
        }

        [Fact]
        public async Task GetOrderDetailByIdAsync_ReturnsOrderDetails()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = JsonContent.Create(new OrderDetails(DateOnly.Parse("2023-01-01"), "Test Order", 1, 100))
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://localhost:7021/")
            };

            var service = new OrderDetailsService(httpClient);

            // Act
            var result = await service.GetOrderDetailByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Order", result.OrderName);
        }

        [Fact]
        public async Task InsertJsonToDatabaseAsync_InsertsOrderDetails()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://localhost:7021/")
            };

            var service = new OrderDetailsService(httpClient);

            // Act
            await service.InsertJsonToDatabaseAsync(DateOnly.Parse("2023-01-01"), "Test Order", 1, 100);

            // Assert
            mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri == new Uri("http://localhost:7021/api/OrderDetails?date=2023-01-01&name=Test%20Order&count=1&cost=100")),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task ModifyJsonFieldInDatabaseAsync_ModifiesOrderDetails()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://localhost:7021/")
            };

            var service = new OrderDetailsService(httpClient);

            // Act
            await service.ModifyJsonFieldInDatabaseAsync(1, DateOnly.Parse("2023-01-02"), "Updated Order", 2, 200);

            // Assert
            mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Put &&
                    req.RequestUri == new Uri("http://localhost:7021/api/OrderDetails?id=1&date=2023-01-02&name=Updated%20Order&count=2&cost=200")),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task DeleteJsonFromDatabaseAsync_DeletesOrderDetails()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://localhost:7021/")
            };

            var service = new OrderDetailsService(httpClient);

            // Act
            await service.DeleteJsonFromDatabaseAsync(1);

            // Assert
            mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Delete &&
                    req.RequestUri == new Uri("http://localhost:7021/api/OrderDetails?id=1")),
                ItExpr.IsAny<CancellationToken>());
        }
    }
}