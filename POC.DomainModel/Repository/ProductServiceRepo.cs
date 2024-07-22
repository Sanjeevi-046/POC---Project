using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using PagedList;
using Poc.CommonModel.Models;
using POC.DomainModel.Models;
using POC.DomainModel.TempModel;
using System.Data;

namespace POC.DomainModel.Repository
{
    public class ProductServiceRepo : IProductRepo
    {
        private readonly DemoProjectContext _context;
        public ProductServiceRepo(DemoProjectContext context)
        {
            _context = context;
        }
        public async Task<PaginationModel> GetAllProducts(int page, string searchName)
        {
            int PageSize = 6;
            var ProductData = await _context.Products.ToListAsync();

            if (!String.IsNullOrEmpty(searchName))
            {
                ProductData = await _context.Products.Where(x => x.Name.Contains(searchName)).ToListAsync();
            }

            var pageData = ProductData.ToPagedList(page, PageSize);
            var count = pageData.PageCount;
            PaginationModel pagination = new PaginationModel
            {
                Page = page,
                Total = count,
                Product = pageData.ToList()
            };
            return pagination;
        }

        public async Task<Product> GetProductById(int id)
        {
            var ProductData = await _context.Products.FindAsync(id);
            return ProductData;
        }
        public async Task<UserValidationResult> AddProduct(Product product)
        {
            var ProductData = await _context.Products.FirstOrDefaultAsync(x => x.Name == product.Name);
            if (ProductData == null)
            {
                byte[] imageFile = product.ProductImage;
                if (product.ProductAvailable > 0)
                {
                    product.IsAvailable = true;
                    product.IsQuantityAvailable = true;
                }
                if (imageFile != null && imageFile.Length > 0)
                {
                    product.ProductImage = imageFile;
                }
                _context.Products.Add(product);
                _context.SaveChanges();
                return new UserValidationResult { IsValid = true, Message = "Product Added Successfully" };
            }
            return new UserValidationResult { IsValid = false, Message = "Product has Already exixted" };
        }

        public async Task<MemoryStream> DownloadExcelFile()
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
                dt.Rows.Add(product.Name, product.Price, product.Description,
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
