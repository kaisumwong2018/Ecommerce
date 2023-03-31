using AdvanceATM.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Model
{
    public class AppDbContext:DbContext
    {
        public DbSet<Customer> customers { get; set; }

        public DbSet<Cart> carts { get; set; }
        public DbSet<Address> Address { get; set; }
        public DbSet<OrderDetail> orderDetails { get; set; }
     
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=DESKTOP-M3BPRVG\SQLEXPRESS;Database=ALTHEALEOW;Trusted_Connection=True;");
        }
    }
}
