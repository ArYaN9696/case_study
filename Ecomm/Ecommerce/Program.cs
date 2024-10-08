using Ecommerce.Main;
using Ecommerce.Repository;
using Ecommerce.Repository.Interface;
using Ecommerce.Service;

namespace Ecommerce
{
    internal class Program
    {
        static void Main(string[] args)
        {

            IOrderProcessorRepository orderProcessor = new OrderProcessorRepositoryImpl();
            EComService orderService = new EComService(orderProcessor);


            MainModule menu = new MainModule(orderService);
            menu.ShowMenu();
        }
    }
}

