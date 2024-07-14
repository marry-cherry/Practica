using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleResult
{
    public class OrderDetails
    {
        public int OrderId { get; set; }
        public DateOnly Date { get; set; }
        public string OrderName { get; set; }
        public int Count { get; set; }
        public int Cost { get; set; }

        public OrderDetails()
        {
        }

        public OrderDetails(DateOnly date, string orderName, int count, int cost)
        {
            Date = date;
            OrderName = orderName;
            Count = count;
            Cost = cost;
        }
    }
}
