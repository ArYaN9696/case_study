
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

namespace Ecommerce.Repository
{
    public class OrderProcessorRepositoryImpl:IOrderProcessorRepository
    {
        private readonly string _connectionString;

        public OrderProcessorRepositoryImpl()
        {
            _connectionString = DbConnUtil.GetConnString();
        }

        // Create a new product
        public bool CreateProduct(Products product)
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            string query = "INSERT INTO products (name, price, description, stockQuantity) VALUES (@Name, @Price, @Description, @StockQuantity)";
            SqlCommand cmd = new SqlCommand(query, conn);
           // cmd.Parameters.AddWithValue("@ProductId", product.ProductId);
            cmd.Parameters.AddWithValue("@Name", product.Name);
            cmd.Parameters.AddWithValue("@Price", product.Price);
            cmd.Parameters.AddWithValue("@Description", product.Description);
            cmd.Parameters.AddWithValue("@StockQuantity", product.StockQuantity);

            conn.Open();
            int result = cmd.ExecuteNonQuery();
            conn.Close();
            return result > 0;
        }

        // Create a new customer
        public bool CreateCustomer(Customers customer)
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            string query = "INSERT INTO customers ( name, email, password) VALUES (@Name, @Email, @Password)";
            SqlCommand cmd = new SqlCommand(query, conn);
           // cmd.Parameters.AddWithValue("@CustomerId", customer.CustomerId);
            cmd.Parameters.AddWithValue("@Name", customer.Name);
            cmd.Parameters.AddWithValue("@Email", customer.Email);
            cmd.Parameters.AddWithValue("@Password", customer.Password);
             
            conn.Open();
            int result = cmd.ExecuteNonQuery();
            conn.Close();
            return result > 0;
        }

        // Delete a product by its ID
        public bool DeleteProduct(int productId)
        {
            if (!DoesProductExist(productId))
            {
                throw new ProductNotFoundException();
            }
            SqlConnection conn = new SqlConnection(_connectionString);
            string query = "DELETE FROM products WHERE product_id = @ProductId";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@ProductId", productId);

            conn.Open();
            int result = cmd.ExecuteNonQuery();
            conn.Close();
            return result > 0;
        }

        // Delete a customer by their ID
        public bool DeleteCustomer(int customerId)
        {
            if (!DoesCustomerExist(customerId))
            {
                throw new CustomerNotFoundException();
            }
            SqlConnection conn = new SqlConnection(_connectionString);
            string query = "DELETE FROM customers WHERE customer_id = @CustomerId";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@CustomerId", customerId);

            conn.Open();
            int result = cmd.ExecuteNonQuery();
            conn.Close();
            return result > 0;
        }

        // Add a product to the cart
        public bool AddToCart(Customers customer, Products product, int quantity)
        {
            if (!DoesCustomerExist(customer.CustomerId))
            {
                throw new CustomerNotFoundException();
            }

            if (!DoesProductExist(product.ProductId))
            {
                throw new ProductNotFoundException();
            }
            SqlConnection conn = new SqlConnection(_connectionString);
            string query = "INSERT INTO cart ( customer_id, product_id, quantity) VALUES (@CustomerId, @ProductId, @Quantity)";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@CustomerId", customer.CustomerId);
            cmd.Parameters.AddWithValue("@ProductId", product.ProductId);
            cmd.Parameters.AddWithValue("@Quantity", quantity);

            conn.Open();
            int result = cmd.ExecuteNonQuery();
            conn.Close();
            return result > 0;
        }

        // Remove a product from the cart
        public bool RemoveFromCart(Customers customer, Products product)
        {
            if (!DoesCustomerExist(customer.CustomerId))
            {
                throw new CustomerNotFoundException();
            }

            if (!DoesProductExist(product.ProductId))
            {
                throw new ProductNotFoundException();
            }
            SqlConnection conn = new SqlConnection(_connectionString);
            string query = "DELETE FROM cart WHERE customer_id = @CustomerId AND product_id = @ProductId";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@CustomerId", customer.CustomerId);
            cmd.Parameters.AddWithValue("@ProductId", product.ProductId);

            conn.Open();
            int result = cmd.ExecuteNonQuery();
            conn.Close();
            return result > 0;
        }

        // Get all products in the customer's cart
        public List<Products> GetAllFromCart(Customers customer)
        {
            if (!DoesCustomerExist(customer.CustomerId))
            {
                throw new CustomerNotFoundException();
            }
            List<Products> products = new List<Products>();
            SqlConnection conn = new SqlConnection(_connectionString);
            if(!DoesCustomerExist(customer.CustomerId))
            {
                throw new CustomerNotFoundException();
            }
            string query = "SELECT p.product_id,p.name,p.price,p.description, c.quantity AS CartQuantity  FROM cart c INNER JOIN products p ON c.product_id = p.product_id WHERE c.customer_id = @CustomerId";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@CustomerId", customer.CustomerId);

            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                products.Add(new Products
                {
                    ProductId = (int)reader["product_id"],
                    Name = (string)reader["name"],
                    Price = (int)reader["price"],
                    Description = (string)reader["description"],
                    StockQuantity = (int)reader["CartQuantity"]
                });
            }
            reader.Close();
            conn.Close();
            return products;

           
        }
        public void PlaceOrderForCart(Customers customer)
        {
            List<Products> cartProducts = GetAllFromCart(customer);
            if (cartProducts.Count == 0)
            {
                Console.WriteLine("Cannot place order. Cart is empty.");
                return; 
            }

            List<Dictionary<Products, int>> productsWithQuantity = new List<Dictionary<Products, int>>();
            var productQuantityMap = new Dictionary<Products, int>();

            foreach (var product in cartProducts)
            {
                productQuantityMap.Add(product, product.StockQuantity); 
            }

            productsWithQuantity.Add(productQuantityMap);

            Console.Write("Enter Shipping Address: ");
            string shippingAddress = Console.ReadLine();

            // Call PlaceOrder method
            bool isOrderPlaced = PlaceOrder(customer, productsWithQuantity, shippingAddress);

            if (isOrderPlaced)
            {
                Console.WriteLine("Order placed successfully!");

                ClearCart(customer.CustomerId);
            }
            else
            {
                Console.WriteLine("Failed to place order.");
            }
        }
        public void ClearCart(int customerId)
        {
            string query = "DELETE FROM cart WHERE customer_id = @CustomerId";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@CustomerId", customerId);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Place an order and update the orders and order_items tables
        public bool PlaceOrder(Customers customer, List<Dictionary<Products, int>> productsWithQuantity, string shippingAddress)
        {
           
            SqlConnection conn = new SqlConnection(_connectionString);
            if (!DoesCustomerExist(customer.CustomerId))
            {
                throw new CustomerNotFoundException();
            }
            // Insert into orders table
            string query = @"
        INSERT INTO orders (customer_id, order_date, total_price, shipping_address) 
        VALUES (@CustomerId, @OrderDate, @TotalPrice, @ShippingAddress);
        SELECT SCOPE_IDENTITY();";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@CustomerId", customer.CustomerId);
            cmd.Parameters.AddWithValue("@OrderDate", DateTime.Now);
            cmd.Parameters.AddWithValue("@TotalPrice", CalculateTotalPrice(productsWithQuantity));
            cmd.Parameters.AddWithValue("@ShippingAddress", shippingAddress);

            conn.Open();
           

            int orderId = Convert.ToInt32(cmd.ExecuteScalar());
            conn.Close();

            foreach (var productWithQuantity in productsWithQuantity)
            {
                foreach (var product in productWithQuantity)
                {
                    string orderItemQuery = "INSERT INTO order_items (order_id, product_id, quantity) VALUES (@OrderId, @ProductId, @Quantity)";
                    SqlCommand orderItemCmd = new SqlCommand(orderItemQuery, conn);
                    orderItemCmd.Parameters.AddWithValue("@OrderId", orderId);
                    orderItemCmd.Parameters.AddWithValue("@ProductId", product.Key.ProductId);
                    orderItemCmd.Parameters.AddWithValue("@Quantity", product.Value);

                    conn.Open();
                    orderItemCmd.ExecuteNonQuery();
                    conn.Close();

                    string updateStockQuery = "UPDATE products SET stockQuantity = stockQuantity - @Quantity WHERE product_id = @ProductId";
                    SqlCommand updateStockCmd = new SqlCommand(updateStockQuery, conn);
                    updateStockCmd.Parameters.AddWithValue("@Quantity", product.Value);
                    updateStockCmd.Parameters.AddWithValue("@ProductId", product.Key.ProductId);

                    conn.Open();
                    updateStockCmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            
            return true;
        }

        // Get all orders for a specific customer
        public List<Orders> GetOrdersByCustomer(int customerId)
        {
            List<Orders> ord = new List<Orders>();
            if (!DoesCustomerExist(customerId))
            {
                throw new CustomerNotFoundException();
            }
            SqlConnection conn = new SqlConnection(_connectionString);
            string query = "SELECT * FROM orders WHERE customer_id = @CustomerId";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@CustomerId", customerId);

            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ord.Add(new Orders
                {
                    OrderId = (int)reader["order_id"],
                    CustomerId = (int)reader["customer_id"],
                    OrderDate = (DateTime)reader["order_date"],
                    TotalPrice = (int)reader["total_price"],
                    ShippingAddress = (string)reader["shipping_address"]
                });
            }
            reader.Close();
            conn.Close();
            return ord;
        }

        private decimal CalculateTotalPrice(List<Dictionary<Products, int>> productsWithQuantity)
        {
            decimal total = 0;
            foreach (var productWithQuantity in productsWithQuantity)
            {
                foreach (var product in productWithQuantity)
                {
                    total += product.Key.Price * product.Value;
                }
            }
            return total;
        }

        private bool DoesCustomerExist(int customerId)
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            string query = "SELECT COUNT(1) FROM customers WHERE customer_id = @CustomerId";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@CustomerId", customerId);

            conn.Open();
            int count = (int)cmd.ExecuteScalar(); 
            conn.Close();

            return count > 0; 
        }

        private bool DoesProductExist(int productId)
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            string query = "SELECT COUNT(1) FROM products WHERE product_id = @ProductId";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@ProductId", productId);

            conn.Open();
            int count = (int)cmd.ExecuteScalar(); 
            conn.Close();

            return count > 0; 
        }

        public Customers GetCustomerByEmail(string email)
        {
            Customers customer = null;

            string query = "SELECT customer_id, name, email, password FROM customers WHERE email = @Email";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Email", email);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    customer = new Customers
                    {
                        CustomerId = (int)reader["customer_id"],
                        Name = (string)reader["name"],
                        Email = (string)reader["email"],
                        Password = (string)reader["password"]
                    };
                }

                reader.Close();
            }

            return customer;
        }
    }
}
