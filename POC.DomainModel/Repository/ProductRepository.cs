using AutoMapper;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using PagedList;
using Poc.CommonModel.Models;
using POC.CommonModel.Models;
using POC.DataLayer.Models;
using POC.DataLayer.TempModel;
using System.Data;
using System.Text;

namespace POC.DataLayer.Repository
{
    public class ProductRepository : IProductRepo
    {
        private readonly DemoProjectContext _context;
        private readonly IMapper _mapper;
        public ProductRepository(DemoProjectContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<CommonPaginationModel> GetAllProducts(int page, string searchName)
        {
            int PageSize = 6;
            //EntityFramework sql 
            var ProductData = await _context.Products.FromSqlRaw("EXEC GetProducts").ToListAsync();

            if (!string.IsNullOrEmpty(searchName))
            {
                ProductData = await _context.Products.FromSqlRaw("EXEC GetProductsByName {0}", searchName).ToListAsync();
            }

            var pageData = ProductData.ToPagedList(page, PageSize);
            var count = pageData.PageCount;
            PaginationModel pagination = new PaginationModel
            {
                Page = page,
                Total = count,
                Product = pageData.ToList()
            };
            return _mapper.Map<CommonPaginationModel>(pagination); 
        }

        public async Task<CommonProductModel> GetProductById(int id)
        {
            //var ProductData = await _context.Products.FindAsync(id);
            var ProductData = await _context.Products.FromSqlInterpolated($"exec GetProductsById {id}").ToListAsync();
            return _mapper.Map<CommonProductModel>(ProductData[0]);
        }
        public async Task<UserValidationResult> AddProduct(CommonProductModel product)
        {

            var commonProductData = _mapper.Map<Product>(product);
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
                _context.Products.Add(commonProductData);
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

        public async Task<MemoryStream> DownloadHtmlReport()
        {
            var products = await _context.Products.ToListAsync();

            var sb = new StringBuilder();
            sb.AppendLine("<html><head><style>");
            sb.AppendLine("table { border-collapse: collapse; width: 100%; }");
            sb.AppendLine("th, td { border: 1px solid black; padding: 8px; text-align: left; }");
            sb.AppendLine("th { background-color: #f2f2f2; }");
            sb.AppendLine("</style></head><body>");
            sb.AppendLine("<h1>Product Report</h1>");
            sb.AppendLine("<table><tr><th>Name</th><th>Price</th><th>Description</th></tr>");

            foreach (var product in products)
            {
                sb.AppendLine($"<tr><td>{product.Name}</td><td>{product.Price:C}</td><td>{product.Description}</td></tr>");
            }

            sb.AppendLine("</table></body></html>");

            var htmlContent = sb.ToString();
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(htmlContent));
            return stream;
        }


    }
}
