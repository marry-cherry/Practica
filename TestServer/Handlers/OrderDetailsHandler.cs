using Microsoft.EntityFrameworkCore;
using TestServer.Core;

namespace TestServer.Handlers
{
    public static class OrderDetailsHandler
    {
        public static async Task AddToDb(OrderDetails orderDetails, AppDbContext dbContext)
        {
            await dbContext.OrderTable.AddAsync(orderDetails);
            await dbContext.SaveChangesAsync();
        }

        public static async Task ChangeOrder(int id, DateOnly? date, string? name, int? count, int? cost, AppDbContext dbContext)
        {
            OrderTable orderDetails = await dbContext.OrderTable.FirstOrDefaultAsync(x => x.OrderId == id);
            if (date != null) orderDetails.Date = (DateOnly)date;
            if (name != null) orderDetails.OrderName = (string)name;
            if (count != null) orderDetails.Count = (int)count;
            if (cost != null) orderDetails.Cost = (int)cost;

            dbContext.Update(orderDetails);
            await dbContext.SaveChangesAsync();
        }

        public static async Task DeleteOrder(int id, AppDbContext dbContext)
        {
            OrderTable orderDetails = await dbContext.OrderTable.FirstOrDefaultAsync(x => x.OrderId == id);
            dbContext.OrderTable.Remove(orderDetails);
            await dbContext.SaveChangesAsync();
        }
    }
}
