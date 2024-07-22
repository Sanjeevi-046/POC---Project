using AutoMapper;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using PagedList;
using Poc.CommonModel.Models;
using POC.CommonModel.Models;
using POC.DomainModel.Models;
using POC.DomainModel.TempModel;
using System.Data;

namespace POC.DomainModel.Repository
{
    public class ProductServiceRepo : IProductRepo
    {
        private readonly DemoProjectContext _context;
        private readonly IMapper _mapper;
        public ProductServiceRepo(DemoProjectContext context , IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<CommonPaginationModel> GetAllProducts(int page, string searchName)
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
            return _mapper.Map<CommonPaginationModel>(pagination);
        }

        public async Task<CommonProductModel> GetProductById(int id)
        {
            var ProductData = await _context.Products.FindAsync(id);
            return _mapper.Map<CommonProductModel>(ProductData);
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

    }
}
