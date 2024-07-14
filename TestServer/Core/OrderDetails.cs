using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace TestServer.Core
{
    public class OrderDetails : OrderTable
    {
        public OrderDetails(DateOnly date, string name, int count, int cost) 
        {
            Date = date;
            OrderName = name;
            Count = count;
            Cost = cost;
        }
    }
}
