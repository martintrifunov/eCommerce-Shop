using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shop.Web.Models.Domain;
using Shop.Web.Models.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public virtual DbSet<ProductInShoppingCart> ProductInShoppingCarts { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //ID Generation
            builder.Entity<Product>().Property(z => z.Id).ValueGeneratedOnAdd();
            builder.Entity<ShoppingCart>().Property(z => z.Id).ValueGeneratedOnAdd();
            
            //Set Primary Key
            builder.Entity<ProductInShoppingCart>().HasKey(z => new { z.ProductId, z.ShoppingCartId });
            builder.Entity<ProductInOrder>().HasKey(z => new { z.ProductId, z.OrderId });

            //Relations n->n
            builder.Entity<ProductInShoppingCart>()
                .HasOne(z => z.Product)
                .WithMany(z => z.ProductInShoppingCarts)
                .HasForeignKey(z => z.ShoppingCartId);

            builder.Entity<ProductInShoppingCart>()
                .HasOne(z => z.ShoppingCart)
                .WithMany(z => z.ProductInShoppingCarts)
                .HasForeignKey(z => z.ProductId);

            builder.Entity<ProductInOrder>()
                .HasOne(z => z.SelectedProduct)
                .WithMany(z => z.Orders)
                .HasForeignKey(z => z.ProductId);

            builder.Entity<ProductInOrder>()
                .HasOne(z => z.UserOrder)
                .WithMany(z => z.Products)
                .HasForeignKey(z => z.OrderId);

            //Relations 1->1
            builder.Entity<ShoppingCart>()
                .HasOne<ApplicationUser>(z => z.Owner).WithOne(z => z.UserCart).HasForeignKey<ShoppingCart>(z => z.OwnerId);

            
        }
    }
}
