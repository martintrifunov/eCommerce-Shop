using Shop.Web.Interface;
using Shop.Web.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Web.Implementation
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository productRepository;

        public ProductService(IProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }

        public Product GetProduct(BaseEntity model)
        {
            return this.productRepository.GetProduct(model);
        }

        public List<Product> GetAllProducts()
        {
            return this.productRepository.GetAllProducts();
        }
    }
}
