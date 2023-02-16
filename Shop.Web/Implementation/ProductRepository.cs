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
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext context;
        private DbSet<Product> entities;

        public ProductRepository(ApplicationDbContext context)
        {
            this.context = context;
            entities = context.Set<Product>();
        }

        public Product GetProduct(BaseEntity model)
        {
            return entities.SingleOrDefaultAsync(z => z.Id.Equals(model.Id)).Result;
        }

        public List<Product> GetAllProducts()
        {
            return entities.ToListAsync().Result;
        }
    }
}
