using Shop.Web.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Shop.Web.Models.DTO
{
    public class AddToShoppingCartDTO
    {
        public Product SelectedProduct { get; set; }
        public Guid SelectedProductId { get; set; }
        public int Quantity { get; set; }
    }
}
