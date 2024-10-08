using Ecommerce.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Repository.Interface
{
    public interface IOrderProcessorRepository
    {
        bool CreateProduct(Products product);
        bool CreateCustomer(Customers customer);
        bool DeleteProduct(int productId);
        bool DeleteCustomer(int customerId);
        bool AddToCart(Customers customer, Products product, int quantity);
        bool RemoveFromCart(Customers customer, Products product);
        List<Products> GetAllFromCart(Customers customer);
        bool PlaceOrder(Customers customer, List<Dictionary<Products, int>> productsWithQuantity, string shippingAddress);
        List<Orders> GetOrdersByCustomer(int customerId);
        Customers GetCustomerByEmail(string email);
        void ClearCart(int customerId);
        void PlaceOrderForCart(Customers customer);
    }
}
