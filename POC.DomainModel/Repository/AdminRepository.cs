using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using PagedList;
using Poc.CommonModel.Models;
using POC.CommonModel.Models;
using POC.DataLayer.Context;
using POC.DataLayer.Models;
using System.Data;
using System.Text;


namespace POC.DataLayer.Repository
{
    public class AdminRepository : IAdminRepo
    {
        private readonly DemoProjectContext _context;
        private readonly AdminDbContext _adminContext;
        public AdminRepository(DemoProjectContext Context, AdminDbContext adminContext)
        {
            _context = Context;
            _adminContext = adminContext;
        }

        public async Task<CommonPaginationModel> GetOrderAsync(string sortColumnName = "Order ID", string sortOrder = "asc", string Status = "", string SearchName = "", DateTime? fromDate = null, DateTime? toDate = null, int pageNumber = 1)
        {
            int pageSize = 10;
            var orderDetails = await _adminContext.AdminOrderDetails.FromSqlRaw("EXEC GetOrderDetails").ToListAsync();

            if (!string.IsNullOrEmpty(Status))
            {
                if (Status.ToLower() == "delivered")
                {
                    orderDetails = await _adminContext.AdminOrderDetails.FromSqlRaw("EXEC GetDeliveredOrder").ToListAsync();
                }
                else if (Status.ToLower() == "cancelled")
                {
                    orderDetails = await _adminContext.AdminOrderDetails.FromSqlRaw("EXEC GetCancelledOrder").ToListAsync();
                }
            }

            if (fromDate.HasValue)
            {
                orderDetails = orderDetails.Where(x => x.OrderDate >= fromDate.Value).ToList();
            }

            if (toDate.HasValue)
            {
                orderDetails = orderDetails.Where(x => x.OrderDate <= toDate.Value).ToList();
            }

            if (!string.IsNullOrEmpty(SearchName))
            {
                orderDetails = orderDetails.Where(x => x.UserName.Contains(SearchName, StringComparison.OrdinalIgnoreCase) ||
                                                       x.OrderId.ToString().Contains(SearchName, StringComparison.OrdinalIgnoreCase))
                                                       .ToList();
            }

            if (!string.IsNullOrEmpty(sortColumnName))
            {
                bool isAscending = string.Equals(sortOrder, "asc", StringComparison.OrdinalIgnoreCase);

                switch (sortColumnName.ToLower().Replace(" ",""))
                {
                    case "status":
                        orderDetails = isAscending ? orderDetails.OrderBy(x => x.IsDelivered).ToList()
                                                   : orderDetails.OrderByDescending(x => x.IsDelivered).ToList();
                        break;

                    case "username":
                        orderDetails = isAscending ? orderDetails.OrderBy(x => x.UserName).ToList()
                                                   : orderDetails.OrderByDescending(x => x.UserName).ToList();
                        break;

                    case "date":
                        orderDetails = isAscending ? orderDetails.OrderBy(x => x.OrderDate).ToList()
                                                   : orderDetails.OrderByDescending(x => x.OrderDate).ToList();
                        break;


                    default: 
                        orderDetails = isAscending ? orderDetails.OrderBy(x => x.OrderId).ToList()
                                                   : orderDetails.OrderByDescending(x => x.OrderId).ToList();
                        break;
                }
            }

            var pagedOrders = orderDetails.ToPagedList(pageNumber, pageSize);

            var commonPagination = new CommonPaginationModel
            {
                Page = pageNumber,
                Total = pagedOrders.PageCount,
                AdminOrderDetails = pagedOrders.ToList()
            };

            return commonPagination;
        }

        public async Task<MemoryStream> GetInvoicePdfAsync(int orderID)
        {

            var orders = await _context.Orders.Where(o => o.OrderId == orderID).FirstOrDefaultAsync();
            var login = await _context.Logins.Where(l=>l.Id == orders.UserId).FirstOrDefaultAsync();
            var addresses = _context.AddressDetails.Where(ad => ad.UserId == orders.UserId).ToList();
            var products = _context.Products.Where(p=>p.ProductId==orders.ProductId).ToList();  


            var query = from o in _context.Orders
                        join l in _context.Logins on o.UserId equals l.Id
                        join p in _context.Products on o.ProductId equals p.ProductId
                        join ad in _context.AddressDetails on o.UserId equals ad.UserId
                        where o.OrderId == orderID && o.IsDelivered == true 
                        select new InvoiceModel
                        {
                            InvoiceNumber = o.OrderId.ToString(),
                            CompanyName = "EKart",
                            CompanyAddress = "123 Street, Coimbatore, India",
                            CompanyPhone = "123-456-2345",
                            BillToName = l.Name,
                            BillToAddress = $"{ad.AreaStreet}, {ad.City}, {ad.State}, {ad.Country}, {ad.Pincode}",
                            BillToPhone = ad.MobileNumber,
                            InvoiceDate = o.OrderDate.ToString("MM/dd/yyyy"),
                            Subtotal = o.OrderPrice,
                            TaxRate = 5,
                            TaxAmount = o.OrderPrice * 5 / 100,
                            Total = (o.OrderPrice * o.ProductQuantity) + ((o.OrderPrice * o.ProductQuantity) * 5 / 100),
                            Items = new List<InvoiceItemModel>
                            {
                                new InvoiceItemModel
                                {
                                    Description = p.Name,
                                    Quantity = o.ProductQuantity,
                                    UnitPrice = p.Price,
                                }
                            }
                        };

            var invoice = await query.FirstOrDefaultAsync();

            if (invoice == null)
                return null;

            // Create HTML string for invoice
            var sb = new StringBuilder();
            sb.Append("<html><body>");
            sb.Append("<table width='100%' cellspacing='0' cellpadding='2'>");

            // Header Section
            sb.Append("<tr><td align='center' colspan='2'><b>Invoice</b></td></tr>");
            sb.Append("<tr><td colspan='2'></td></tr>");
            sb.Append("<tr><td><b>Order No: </b>");
            sb.Append(invoice.InvoiceNumber);
            sb.Append("</td><td align='right'><b>Date: </b>");
            sb.Append(invoice.InvoiceDate);
            sb.Append("</td></tr>");
            sb.Append("<tr><td colspan='2'><b>Company Name: </b>");
            sb.Append(invoice.CompanyName);
            sb.Append("</td></tr>");
            sb.Append("</table>");
            sb.Append("<br />");

            // Billing Address
            sb.Append("<div style='margin-top: 20px;'>");
            sb.Append("<h5 style='font-size: .8em; font-weight: 700; text-transform: uppercase; letter-spacing: 2px; color: #1779ba;'>Billing Information</h5>");
            sb.Append("<p>");
            sb.Append($" Name :{invoice.BillToName}<br>");
            sb.Append($"{invoice.BillToAddress}<br>");
            sb.Append($"Phone: {invoice.BillToPhone}");
            
            sb.Append("</p>");
            sb.Append("</div>");

            // Generate Invoice (Bill) Items Grid
            sb.Append("<table width='100%' border='1' cellspacing='0' cellpadding='2' style='border-collapse: collapse;'>");
            sb.Append("<tr color: #ffffff;'>");
            sb.Append("<th style='padding: 8px;'>Product Name</th>");
            sb.Append("<th style='padding: 8px;'>Unit Price</th>");
            sb.Append("<th style='padding: 8px;'>Quantity</th>");
            sb.Append("<th style='padding: 8px;'>Total Price</th>");
            sb.Append("</tr>");
            decimal subtotal = 0;
            foreach (var item in invoice.Items)
            {
                decimal totalPrice = item.UnitPrice * item.Quantity;
                subtotal += totalPrice;

                sb.Append("<tr>");
                sb.Append("<td style='padding: 8px;'>");
                sb.Append(item.Description);
                sb.Append("</td>");
                sb.Append("<td style='padding: 8px;'>");
                sb.Append(item.UnitPrice.ToString("C"));
                sb.Append("</td>");
                sb.Append("<td style='padding: 8px;'>");
                sb.Append(item.Quantity);
                sb.Append("</td>");
                sb.Append("<td style='padding: 8px;'>");
                sb.Append(totalPrice.ToString("C"));
                sb.Append("</td>");
                sb.Append("</tr>");
            }
            sb.Append("</table>");
            sb.Append("<br />");

            // GST and Total Amount
            decimal taxAmount = invoice.TaxAmount; // 5% GST
            decimal totalAmount = invoice.Total;

            sb.Append("<table width='100%' cellspacing='0' cellpadding='2'>");
            sb.Append("<tr>");
            sb.Append("<td align='right' style='padding: 8px;'><b>Subtotal</b></td>");
            sb.Append("<td style='padding: 8px;'>");
            sb.Append(subtotal.ToString("C"));
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td align='right' style='padding: 8px;'><b>GST (5%)</b></td>");
            sb.Append("<td style='padding: 8px;'>");
            sb.Append(taxAmount.ToString("C"));
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td align='right' style='padding: 8px;'><b>Total</b></td>");
            sb.Append("<td style='padding: 8px;'>");
            sb.Append(totalAmount.ToString("C"));
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("</table>");

            sb.Append("</body></html>");

            var htmlContent = sb.ToString();
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(htmlContent));
            return stream;
        }

        public async Task<MemoryStream> DownloadExcel()
        {
            var orderDetails = await _adminContext.AdminOrderDetails.FromSqlRaw("EXEC GetOrderDetails").ToListAsync();

            DataTable dt = new DataTable("Products");
            dt.Columns.AddRange(new DataColumn[] {
                new DataColumn("OrderId"),
                new DataColumn("UserName"),
                new DataColumn("OrderPrice"),
                new DataColumn("Date"),
                new DataColumn("Product Name"),
                new DataColumn("ProductPrice"),
                new DataColumn("ProductQuantity"),
                new DataColumn("Status")

            });

            foreach(var order in orderDetails)
            {
                dt.Rows.Add(order.OrderId, order.UserName, order.OrderPrice,
                           order.OrderDate,order.ProductName,order.ProductPrice,order.ProductQuantity,order.IsDelivered?"Delivered":"Cancelled");
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
