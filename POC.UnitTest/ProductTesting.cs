using Moq;
using POC.DataAccess.Service;
using POC.DomainModel.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC.UnitTest
{
    public class ProductTesting
    {
        private readonly ProductService _productService;
        private readonly Mock<IProductRepo> _productMock ; 

        public ProductTesting()
        {
            _productMock = new Mock<IProductRepo>();
            _productService = new ProductService(_productMock.Object);
        }

    }
}
