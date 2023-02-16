using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Web.Data;
using Shop.Web.Interface;
using Shop.Web.Models.Domain;
using Shop.Web.Models.DTO;
using Shop.Web.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IOrderService orderService;
        private readonly IProductService productService;
        private readonly ApplicationDbContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private DbSet<Product> entities;

        public AdminController(IOrderService orderService, IProductService productService, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            this.orderService = orderService;
            this.productService = productService;
            this.context = context;
            this.userManager = userManager;
            entities = context.Set<Product>();
        }

        [HttpGet("[action]")]
        public List<Order> GetAllActiveOrders()
        {
            return this.orderService.GetAllOrders();
        }

        [HttpPost("[action]")]
        public Order GetOrder(BaseEntity model)
        {
            return this.orderService.GetOrderDetails(model);
        }

        [HttpGet("[action]")]
        public List<Product> GetAllActiveProducts()
        {
            return this.productService.GetAllProducts();
        }

        [HttpPost("[action]")]
        public async Task<bool> ImportProducts(List<Product> model)
        {
            bool status = true;

            foreach (var item in model)
            {
                var product = new Product 
                {
                    Id = Guid.NewGuid(),
                    Name = item.Name,
                    Image = item.Image,
                    Description = item.Description,
                    Rating = item.Rating,
                    Price = item.Price,
                    Category = item.Category
                };
                context.Add(product);
                await context.SaveChangesAsync();
            }

            return status;
        }

        [HttpPost("[action]")]
        public async Task<bool> DeleteActiveProduct(BaseEntity model)
        {
            bool status = true;
            var itemToDelete = entities.SingleOrDefaultAsync(z => z.Id.Equals(model.Id)).Result;

            context.Remove(itemToDelete);
            await context.SaveChangesAsync();

            return status;
        }

        [HttpPost("[action]")]
        public Product GetProduct(BaseEntity model)
        {
            return this.productService.GetProduct(model);
        }

        [HttpPost("[action]")]
        public async Task<bool> EditProduct([Bind("Id,Name,Image,Description,Rating,Price,Category")] Product model)
        {
            bool status = true;

            var product = new Product 
            { 
                Id = model.Id,
                Name = model.Name,
                Image = model.Image,
                Description = model.Description,
                Rating = model.Rating,
                Price = model.Price,
                Category = model.Category
            };

            context.Update(product);
            await context.SaveChangesAsync();

            return status;
        }

        [HttpPost("[action]")]
        public bool ImportUsers(List<UserRegisterDTO> model)
        {
            bool status = true;

            foreach (var item in model)
            {
                var userCheck = userManager.FindByEmailAsync(item.Email).Result;

                if (userCheck == null)
                {
                    var user = new ApplicationUser
                    {
                        FirstName = item.Name,
                        LastName = item.LastName,
                        UserName = item.Email,
                        NormalizedUserName = item.Email,
                        Email = item.Email,
                        EmailConfirmed = true,
                        UserCart = new ShoppingCart()
                    };
                    var result = userManager.CreateAsync(user, item.Password).Result;

                    status = status && result.Succeeded;
                }
                else continue;
            }

            return status;
        }
    }
}
