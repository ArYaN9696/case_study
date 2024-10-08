using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Model
{
    public class Order_items
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; } 
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        // Default constructor
        public Order_items() { }

        // Parameterized constructor
        public Order_items(int orderItemId, int orderId, int productId, int quantity)
        {
            OrderItemId = orderItemId;
            OrderId = orderId;
            ProductId = productId;
            Quantity = quantity;
        }
    }
}
