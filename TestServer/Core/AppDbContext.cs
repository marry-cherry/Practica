using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestServer.Core
{
    public class AppDbContext : DbContext
    {
        public DbSet<OrderTable> OrderTable { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=Orders.db");
            }
        }
    }

    [Table("OrderTable")]
    public class OrderTable
    {
        [Key]
        public int OrderId { get; set; }

        public DateOnly Date { get; set; }

        public string OrderName { get; set; }

        public int Count { get; set; }

        public int Cost { set; get; }

        private static int _id = 0;

        public OrderTable()
        {
            _id++;
            OrderId = _id;
        }
    }
}
