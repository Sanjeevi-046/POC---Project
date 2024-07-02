using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using POC.DomainModel.Models;
using POC.DomainModel.TempModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC.DataAccess.Service
{
    public class ProductService : IProduct
    {
        private readonly DemoProjectContext _context;

        public ProductService(DemoProjectContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            var data = await _context.Products.ToListAsync();
            return data;
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            var data = await _context.Products.FindAsync(id);
            return data;
        }
        public async Task<UserValidationResult> addProduct (Product product)
        {
            var data = await _context.Products.FirstOrDefaultAsync(x=>x.Name==product.Name);
            if (data == null) 
            {
               if (product.ProductAvailable > 0)
                {
                    product.IsAvailable = true;
                    product.IsQuantityAvailable = true;
                }
            _context.Products.Add(product);
            _context.SaveChanges();
                return new UserValidationResult { IsValid = true, Message = "Product Added Successfully"};
            }
            return new UserValidationResult { IsValid = false, Message = "Product has Already exixted" };
        }

        public async Task<MemoryStream> DownloadExcel()
        {
            DataTable dt = new DataTable("Products");
            dt.Columns.AddRange(new DataColumn[] {
                new DataColumn("Name"),
                new DataColumn("Price"),
                new DataColumn("Description"),
                new DataColumn("ProductImage")
        
            });

            var products = await _context.Products.ToListAsync();

            foreach (var product in products)
            {
                dt.Rows.Add( product.Name, product.Price, product.Description,
                            product.ProductImage);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                var stream = new MemoryStream();
                wb.SaveAs(stream);
                stream.Position = 0; 
                return stream;
            }
        }




    }
}
