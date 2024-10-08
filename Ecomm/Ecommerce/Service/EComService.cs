using Ecommerce.Exceptions;
using Ecommerce.Model;
using Ecommerce.Repository.Interface;
using Ecommerce.Util;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Service
{
    internal class EComService
    {
        private readonly IOrderProcessorRepository _orderProcessor;
        

       
        public EComService(IOrderProcessorRepository orderProcessor)
        {
            _orderProcessor = orderProcessor;
        }

        public void RegisterCustomer()
        {
            
            Console.Write("Enter Name: ");
            string name = Console.ReadLine();
            Console.Write("Enter Email: ");
            string email = Console.ReadLine();
            Console.Write("Enter Password: ");
            string password = Console.ReadLine();

            Customers customer = new Customers( name, email, password);
            bool isSuccess = _orderProcessor.CreateCustomer(customer);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(isSuccess ? "Customer registered successfully." : "Failed to register customer.");
            Console.ResetColor();
        }
        public string Login()
        {
            Console.Write("Enter Email: ");
            string email = Console.ReadLine();

            Console.Write("Enter Password: ");
            string password = Console.ReadLine();
            if (IsAdminUser(email, password))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Admin login successful!");
                Console.ResetColor();
                return "Admin"; 
            }

            var customer = _orderProcessor.GetCustomerByEmail(email);

            if (customer != null && customer.Password == password)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Login successful!");
                Console.ResetColor();
                return "User";
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid email or password.");
                Console.ResetColor();
                return "Invalid";
            }
        }
        public bool IsAdminUser(string email, string password)
        {
            const string adminEmail = "admin@gmail.com";
            const string adminPassword = "admin123";

            return email.Equals(adminEmail) &&
                   password.Equals(adminPassword);
        }

        public void CreateProduct()
        {

            Console.Write("Enter Name: ");
            string name = Console.ReadLine();
            Console.Write("Enter Price: ");
            decimal price = decimal.Parse(Console.ReadLine());
            Console.Write("Enter Description: ");
            string description = Console.ReadLine();
            Console.Write("Enter Stock Quantity: ");
            int stockQuantity = int.Parse(Console.ReadLine());

            Products product = new Products(name, price, description, stockQuantity);
            bool isSuccess = _orderProcessor.CreateProduct(product);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(isSuccess ? "Product created successfully." : "Failed to create product.");
            Console.ResetColor();
        }


        public void DeleteProduct()
        {
            try
            {

                DisplayProductList();
                Console.Write("Enter Product ID to delete: ");

                int productId = int.Parse(Console.ReadLine());
                bool isSuccess = _orderProcessor.DeleteProduct(productId);
                Console.ForegroundColor = ConsoleColor.Yellow;                
                Console.WriteLine("Product deleted successfully.");
                Console.ResetColor();
            }

            catch (ProductNotFoundException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }
        }

        public void AddToCart()
        {
            try
            {

                Console.Write("Enter Customer ID: ");
                int customerId = int.Parse(Console.ReadLine());
                DisplayProductList();
                Console.Write("Enter Product ID: ");
                int productId = int.Parse(Console.ReadLine());

                Console.Write("Enter Quantity: ");
                int quantity = int.Parse(Console.ReadLine());

                Customers customer = new Customers { CustomerId = customerId };
                Products product = new Products { ProductId = productId };

                bool isSuccess = _orderProcessor.AddToCart(customer, product, quantity);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(isSuccess ? "Product added to cart successfully." : "Failed to add product to cart.");
                Console.ResetColor();

            }
            catch (CustomerNotFoundException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }
            catch (ProductNotFoundException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                Console.ResetColor();
            }
        }

        public void ViewCart()
        {
            try
            {

                Console.Write("Enter Customer ID to view cart: ");
                int customerId = int.Parse(Console.ReadLine());
                Customers customer = new Customers { CustomerId = customerId };

                List<Products> cartProducts = _orderProcessor.GetAllFromCart(customer);
                if (cartProducts.Count() == 0)
                    Console.WriteLine("Cart Empty");
                else
                {
                    Console.WriteLine("Cart Products:");
                    foreach (var product in cartProducts)
                    {
                        Console.WriteLine($"- {product.Name} (ID: {product.ProductId}, Price: {product.Price}, Quantity: {product.StockQuantity})");
                    }
                    Console.Write("Do you want to place an order for these items? (yes/no): ");
                    string response = Console.ReadLine();

                    if (response.Equals("yes", StringComparison.OrdinalIgnoreCase))
                    {
                        _orderProcessor.PlaceOrderForCart(customer);
                    }
                }
            }
            catch (Exception ex)
            {

                Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }
           
        }

        public void PlaceOrder()
        {
            try
            {

                Console.Write("Enter Customer ID: ");
                int customerId = int.Parse(Console.ReadLine());
                Console.Write("Enter Shipping Address: ");
                string shippingAddress = Console.ReadLine();
                DisplayProductList();
                List<Dictionary<Products, int>> productsWithQuantity = new List<Dictionary<Products, int>>();
                while (true)
                {
                    Console.Write("Enter Product ID: ");
                    int productId = int.Parse(Console.ReadLine());


                    Console.Write("Enter Product Price: ");
                    decimal price = decimal.Parse(Console.ReadLine());

                    Console.Write("Enter Quantity: ");
                    int quantity = int.Parse(Console.ReadLine());


                    productsWithQuantity.Add(new Dictionary<Products, int>
                {
              { new Products { ProductId = productId, Price = price }, quantity }
                 });


                    Console.Write("Do you want to add another product? (yes/no): ");
                    string response = Console.ReadLine().ToLower();
                    if (response != "yes")
                    {
                        break;
                    }
                }

                Customers customer = new Customers { CustomerId = customerId };
                bool isSuccess = _orderProcessor.PlaceOrder(customer, productsWithQuantity, shippingAddress);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(isSuccess ? "Order placed successfully." : "Failed to place order.");

                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }
        }

        public void ViewCustomerOrder()
        {
            try
            {


                Console.Write("Enter Customer ID to view orders: ");

                int customerId = int.Parse(Console.ReadLine());
                List<Orders> orders = _orderProcessor.GetOrdersByCustomer(customerId);
                Console.WriteLine("Customer Orders:");
                foreach (var order in orders)
                {
                    Console.WriteLine($"- Order ID: {order.OrderId}, Total Price: {order.TotalPrice}, Order Date: {order.OrderDate}");
                }
            }
            catch (Exception ex) 
            {
                Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }
        }

        public void DisplayProductList()
        {
            SqlConnection conn = new SqlConnection(DbConnUtil.GetConnString());

            string query = "SELECT product_id, name, price, stockQuantity FROM products";
            SqlCommand cmd = new SqlCommand(query, conn);

            conn.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            // Format the headers
            Console.ForegroundColor = ConsoleColor.Yellow;
           
            Console.WriteLine("Product List:");
            Console.WriteLine("------------------------------------------------------------");
            Console.WriteLine("{0,-10} {1,-20} {2,-15} {3,-10}", "ID", "Name", "Price", "Stock");
            Console.WriteLine("------------------------------------------------------------");
            Console.ResetColor();

            // Iterate through each row and format the output
            while (reader.Read())
            {
                int productId = (int)reader["product_id"];
                string name = (string)reader["name"];
                int price = (int)reader["price"];  
                int stockQuantity = (int)reader["stockQuantity"];

                // Format the output to align columns
                Console.WriteLine("{0,-10} {1,-20} {2,-15} {3,-10}",
                                  productId,
                                  name,
                                  price,
                                  stockQuantity);
            }

            reader.Close();
            conn.Close();
        }


    }

}

