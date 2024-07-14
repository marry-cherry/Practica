using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleResult
{
    public interface IOrderDetailsService
    {
        Task<List<OrderDetails>> GetOrderDetailsAsync();
        Task<OrderDetails> GetOrderDetailByIdAsync(int id);
        Task InsertJsonToDatabaseAsync(DateOnly date, string name, int count, int cost);
        Task ModifyJsonFieldInDatabaseAsync(int id, DateOnly? date, string? name, int? count, int? cost);
        Task DeleteJsonFromDatabaseAsync(int id);
    }
}
