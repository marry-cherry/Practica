using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleResult
{
    public class OrderDetailsService : IOrderDetailsService
    {
        private readonly HttpClient _httpClient;

        public OrderDetailsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<OrderDetails>> GetOrderDetailsAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<OrderDetails>>("api/OrderDetails");
                if (response != null)
                {
                    Console.WriteLine("Все записи в базе данных:");
                    foreach (var order in response)
                    {
                        Console.WriteLine($"ID: {order.OrderId}, Name: {order.OrderName}, Date: {order.Date}, Count: {order.Count}, Cost: {order.Cost}");
                    }
                }
                else
                {
                    Console.WriteLine("Записей не найдено.");
                }
                return response;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Ошибка при получении данных: {e.Message}");
                return null;
            }
        }

        public async Task<OrderDetails> GetOrderDetailByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/OrderDetails/{id}");
                response.EnsureSuccessStatusCode();

                var orderDetails = await response.Content.ReadFromJsonAsync<OrderDetails>();
                if (orderDetails != null)
                {
                    Console.WriteLine($"Найдена запись: ID: {orderDetails.OrderId}, Name: {orderDetails.OrderName}, Date: {orderDetails.Date}, Count: {orderDetails.Count}, Cost: {orderDetails.Cost}");
                }
                else
                {
                    Console.WriteLine("Запись не найдена.");
                }

                return orderDetails;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Ошибка при получении данных: {e.Message}");
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Произошла ошибка: {e.Message}");
                return null;
            }
        }

        public async Task InsertJsonToDatabaseAsync(DateOnly date, string name, int count, int cost)
        {
            try
            {
                string url = $"api/OrderDetails?date={date.ToString("yyyy-MM-dd")}&name={Uri.EscapeDataString(name)}&count={count}&cost={cost}";
                var response = await _httpClient.PostAsync(url, null);
                response.EnsureSuccessStatusCode();
                Console.WriteLine("Запись успешно вставлена в базу данных.");
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Ошибка при вставке данных: {e.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Произошла ошибка: {e.Message}");
            }
        }

        public async Task ModifyJsonFieldInDatabaseAsync(int id, DateOnly? date, string? name, int? count, int? cost)
        {
            try
            {
                string url = $"api/OrderDetails?id={id}";
                if (date != null) url += $"&date={date.Value.ToString("yyyy-MM-dd")}";
                if (name != null) url += $"&name={Uri.EscapeDataString(name)}";
                if (count != null) url += $"&count={count}";
                if (cost != null) url += $"&cost={cost}";

                var response = await _httpClient.PutAsync(url, null);
                response.EnsureSuccessStatusCode();
                Console.WriteLine("Запись успешно модифицирована.");
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Ошибка при модификации данных: {e.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Произошла ошибка: {e.Message}");
            }
        }

        public async Task DeleteJsonFromDatabaseAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/OrderDetails?id={id}");
                response.EnsureSuccessStatusCode();
                Console.WriteLine("Запись успешно удалена.");
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Ошибка при удалении данных: {e.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Произошла ошибка: {e.Message}");
            }
        }
    }
}
