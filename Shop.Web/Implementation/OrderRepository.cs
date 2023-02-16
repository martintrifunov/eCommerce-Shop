using Microsoft.EntityFrameworkCore;
using Shop.Web.Data;
using Shop.Web.Interface;
using Shop.Web.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Web.Implementation
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext context;
        private DbSet<Order> entities;

        public OrderRepository(ApplicationDbContext context)
        {
            this.context = context;
            entities = context.Set<Order>();
        }

        public List<Order> GetAllOrders()
        {
            return entities
                .Include(z => z.Products)
                .Include(z => z.User)
                .Include("Products.SelectedProduct")
                .ToListAsync().Result;
        }

        public Order GetOrderDetails(BaseEntity model)
        {
            return entities
                .Include(z => z.Products)
                .Include(z => z.User)
                .Include("Products.SelectedProduct")
                .SingleOrDefaultAsync(z => z.Id.Equals(model.Id)).Result;
        }
    }
}
