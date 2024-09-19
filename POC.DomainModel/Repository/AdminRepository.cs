using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using PagedList;
using Poc.CommonModel.Models;
using POC.CommonModel.Models;
using POC.DataLayer.Context;
using POC.DataLayer.Models;
using System.Data;
using System.Text;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using MigraDoc.DocumentObjectModel.Visitors;





namespace POC.DataLayer.Repository
{
    public class AdminRepository : IAdminRepo
    {
        private readonly DemoProjectContext _context;
        private readonly AdminDbContext _adminContext;
        //private readonly IRazorViewToStringRenderer _engine;
       // private readonly IGeneratePdf _generatePdf;


        public AdminRepository(DemoProjectContext Context, AdminDbContext adminContext)//, IGeneratePdf generatePdf
        {
            _context = Context;
            _adminContext = adminContext;
            //_generatePdf = generatePdf;
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


            var document = new Document();
            var section = document.AddSection();
            

            // Add company information
            section.AddParagraph("Invoice", "Heading1");
            section.AddParagraph($"Company: {invoice.CompanyName}");
            section.AddParagraph($"Address: {invoice.CompanyAddress}");
            section.AddParagraph($"Phone: {invoice.CompanyPhone}");
            section.AddParagraph();

            // Add billing information
            section.AddParagraph("Bill To:", "Heading2");
            section.AddParagraph($"Name: {invoice.BillToName}");
            section.AddParagraph($"Address: {invoice.BillToAddress}");
            section.AddParagraph($"Phone: {invoice.BillToPhone}");
            section.AddParagraph();

            // Add invoice details
            section.AddParagraph($"Invoice Number: {invoice.InvoiceNumber}");
            section.AddParagraph($"Date: {invoice.InvoiceDate}");
            section.AddParagraph();

            // Create a table for items
            var table = section.AddTable();
            table.RightPadding = 10;
            table.Style = "Table";
            table.Borders.Width = 0.25;
            table.Borders.Left.Width = 0.5;
            table.Borders.Right.Width = 0.5;
            table.Rows.LeftIndent = 0;
            table.Borders.Width = 0.75;
            table.AddColumn(Unit.FromCentimeter(7)); // Description
            table.AddColumn(Unit.FromCentimeter(4)); // Quantity
            table.AddColumn(Unit.FromCentimeter(4)); // Unit Price
            table.AddColumn(Unit.FromCentimeter(4)); // Total Price

            // Add table header
            var row = table.AddRow();
            row.Cells[0].AddParagraph("Description");
            row.Cells[1].AddParagraph("Quantity");
            row.Cells[2].AddParagraph("Unit Price");
            row.Cells[3].AddParagraph("Total Price");

            // Add items to table
            foreach (var item in invoice.Items)
            {
                row = table.AddRow();
                row.Cells[0].AddParagraph(item.Description);
                row.Cells[1].AddParagraph(item.Quantity.ToString());
                row.Cells[2].AddParagraph(item.UnitPrice.ToString("C"));
                row.Cells[3].AddParagraph((item.Quantity * item.UnitPrice).ToString("C"));
            }

            // Add summary
            section.AddParagraph();
            section.AddParagraph($"Subtotal: {invoice.Subtotal:C}");
            section.AddParagraph($"Tax ({invoice.TaxRate}%): {invoice.TaxAmount:C}");
            section.AddParagraph($"Total: {invoice.Total:C}");

            // Render the document to a PDF file
            var pdfRenderer = new PdfDocumentRenderer(true);
            pdfRenderer.Document = document;
            pdfRenderer.RenderDocument();

            var memoryStream = new MemoryStream();

            pdfRenderer.PdfDocument.Save(memoryStream, false);

            memoryStream.Position = 0;

            return memoryStream;












            //var pdf = WkhtmlDriver.Convert(null,null, htmlContent);
            //var pdfByte = _generatePdf.GetPDF(htmlContent);


            //PdfDocument pdfDocument = PdfGenerator.GeneratePdf(htmlContent, PdfSharp.PageSize.A4);

            //using (var memoryStream = new MemoryStream())
            //{
            //    pdfDocument.Save(memoryStream, false);

            //    memoryStream.Position = 0;
            //    return memoryStream;
            //}

            //string tempHtmlPath = Path.GetTempFileName();
            //File.WriteAllText(tempHtmlPath, htmlContent);

            //string outputPath = "html_to_pdf.pdf";

            //ProcessStartInfo startInfo = new ProcessStartInfo
            //{
            //    FileName = "wkhtmltopdf",
            //    Arguments = $"\"{tempHtmlPath}\" \"{outputPath}\"",
            //    UseShellExecute = false
            //};

            //using (Process process = new Process { StartInfo = startInfo })
            //{
            //    process.Start();
            //    process.WaitForExit();  // Ensure the process completes
            //}

            //File.Delete(tempHtmlPath);


            // Create a new HTML-to-PDF converter
            //var converter = new BasicConverter(new PdfTools());

            //// Create the PDF document
            //var document = new HtmlToPdfDocument
            //{
            //    GlobalSettings = { DocumentTitle = "HTML to PDF", PaperSize = PaperKind.A4 },
            //    Objects = { new ObjectSettings { HtmlContent = htmlContent } }
            //};

            //// Convert the document to a PDF
            //byte[] pdfBytes = converter.Convert(document);
            //memoryStream.Write(pdfBytes, 0, pdfBytes.Length);
            //memoryStream.Position = 0;

            //// Save the PDF to file
            //File.WriteAllBytes("html_to_pdf.pdf", pdfBytes);

            //var memoryStream = new MemoryStream();

           // var renderer = new ChromePdfRenderer();
           // var pdf = renderer.RenderHtmlAsPdf(htmlContent);
            
           // string tempHtmlPath = Path.GetTempFileName();
           // pdf.SaveAs(tempHtmlPath);
           // byte[] byteArray = Encoding.ASCII.GetBytes(tempHtmlPath);
           //memoryStream.Write(byteArray, 0, byteArray.Length);
           // memoryStream.Position = 0;
           
           // return memoryStream;


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
