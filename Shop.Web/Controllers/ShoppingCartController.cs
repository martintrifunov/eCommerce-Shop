using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Web.Data;
using Shop.Web.Models.Domain;
using Shop.Web.Models.DTO;
using Shop.Web.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Shop.Web.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ShoppingCartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loggedInUser = await _context.Users.Where(z => z.Id.Equals(userId))
                .Include(z => z.UserCart)
                .Include(z => z.UserCart.ProductInShoppingCarts)
                .Include("UserCart.ProductInShoppingCarts.Product")
                .FirstOrDefaultAsync();
            var userShoppingCart = loggedInUser.UserCart;
            var productPrice = userShoppingCart.ProductInShoppingCarts.Select(z => new
            {
                Price = z.Product.Price,
                Quantity = z.Quantity
            }).ToList();
            double totalPrice = 0;

            foreach (var item in productPrice)
            {
                totalPrice += (item.Price * item.Quantity);
            }

            ShoppingCartDTO shoppingCartDTOItem = new ShoppingCartDTO
            {
                ProductInShoppingCarts = userShoppingCart.ProductInShoppingCarts.ToList(),
                TotalPrice = totalPrice + 5
            };

            return View(shoppingCartDTOItem);
        }

        public async Task<IActionResult> DeleteProductFromShoppingCart(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loggedInUser = await _context.Users.Where(z => z.Id.Equals(userId))
                .Include(z => z.UserCart)
                .Include(z => z.UserCart.ProductInShoppingCarts)
                .Include("UserCart.ProductInShoppingCarts.Product")
                .FirstOrDefaultAsync();
            var userShoppingCart = loggedInUser.UserCart;

            var itemToDelete = userShoppingCart.ProductInShoppingCarts.Where(z => z.ProductId.Equals(id)).FirstOrDefault();

            userShoppingCart.ProductInShoppingCarts.Remove(itemToDelete);

            _context.Update(userShoppingCart);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "ShoppingCart");
        }

        public async Task<IActionResult> Order()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loggedInUser = await _context.Users.Where(z => z.Id.Equals(userId))
                .Include(z => z.UserCart)
                .Include(z => z.UserCart.ProductInShoppingCarts)
                .Include("UserCart.ProductInShoppingCarts.Product")
                .FirstOrDefaultAsync();
            var userShoppingCart = loggedInUser.UserCart;

            Order orderItem = new Order
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                User = loggedInUser
            };

            _context.Add(orderItem);

            List<ProductInOrder> productInOrders = new List<ProductInOrder>();
            productInOrders = userShoppingCart.ProductInShoppingCarts
                .Select(z => new ProductInOrder 
                {
                    OrderId = orderItem.Id,
                    ProductId = z.Product.Id,
                    SelectedProduct = z.Product,
                    UserOrder = orderItem
                }).ToList();

            foreach(var item in productInOrders)
            {
                _context.Add(item);
            }

            loggedInUser.UserCart.ProductInShoppingCarts.Clear();
            _context.Update(loggedInUser);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "ShoppingCart");
        }
    }
}
