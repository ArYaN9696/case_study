using Ecommerce.Repository.Interface;
using Ecommerce.Repository;
using Ecommerce.Model;
using Ecommerce.Exceptions;

namespace EComm.Tests
{
    public class AppTests
    {
        private IOrderProcessorRepository _orderProcessorRepository;

        [SetUp]
        public void Setup()
        {
            _orderProcessorRepository = new OrderProcessorRepositoryImpl();
        }

        [Test]
        public void Test_Product_Created_Successfully()
        {
            var product = new Products
            {
               
                Name = "Laptop",
                Price = 1000,
                Description="i7 intel core",
                StockQuantity = 10
            };

            bool isProductCreated = _orderProcessorRepository.CreateProduct(product);

            Assert.That(isProductCreated, "Product should be created successfully.");
        }
        [Test]
        public void Test_Product_Added_To_Cart_Successfully()
        {
            var customer = new Customers { CustomerId = 1 };
            var product = new Products { ProductId = 101 };   

            
            bool isAddedToCart = _orderProcessorRepository.AddToCart(customer, product, 2); 

            Assert.That(isAddedToCart, Is.True, "Product should be added to the cart successfully.");
        }

        [Test]
        public void Test_Product_Ordered_Successfully()
        {
            var customer = new Customers { CustomerId = 2 }; 
            var productsWithQuantity = new List<Dictionary<Products, int>>
          {
              new Dictionary<Products, int>
            {
              { new Products { ProductId = 102, Price = 1000 }, 2 }  
            }
          };

            string shippingAddress = "123 forge Street";
            bool isOrderPlaced = _orderProcessorRepository.PlaceOrder(customer, productsWithQuantity, shippingAddress);

            Assert.That(isOrderPlaced, Is.True, "Order should be placed successfully.");
        }
        [Test]
        public void Test_Exception_Thrown_When_Customer_Not_Found()
        {
            var invalidCustomer = new Customers { CustomerId = 999 }; 
            var product = new Products { ProductId = 101 };  

            var ex = Assert.Throws<CustomerNotFoundException>(() =>
                _orderProcessorRepository.AddToCart(invalidCustomer, product, 1)
            );

            Assert.That(ex.Message, Is.EqualTo("Customer not found."));
        }
        [Test]
        public void Test_Exception_Thrown_When_Product_Not_Found()
        {
          
            var customer = new Customers { CustomerId = 1 }; 
            var invalidProduct = new Products { ProductId = 999 };  

            
            var ex = Assert.Throws<ProductNotFoundException>(() =>
                _orderProcessorRepository.AddToCart(customer, invalidProduct, 1)
            );

            
            Assert.That(ex.Message, Is.EqualTo("Product not found."));
        }



    }
}