using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Exceptions
{
    public class ProductNotFoundException:Exception
    {
        public ProductNotFoundException() : base("Product not found.") { }
    }
}
