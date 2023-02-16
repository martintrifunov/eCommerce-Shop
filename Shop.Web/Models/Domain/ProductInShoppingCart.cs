using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Web.Models.Domain
{
    public class ProductInShoppingCart
    {
        public Guid ProductId { get; set; }
        public Guid ShoppingCartId { get; set; }
        public virtual Product Product { get; set; }
        public virtual ShoppingCart ShoppingCart { get; set; }
        public int Quantity { get; set; }
    }
}
