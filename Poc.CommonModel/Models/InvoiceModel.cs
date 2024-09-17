namespace POC.CommonModel.Models
{
    public class InvoiceModel
    {
        public string InvoiceNumber { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyPhone { get; set; }
        public string BillToName { get; set; }
        public string BillToAddress { get; set; }
        public string BillToPhone { get; set; }
        public string InvoiceDate { get; set; }
        public string Description { get; set; }
        public decimal Subtotal { get; set; }
        public decimal TaxRate { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal Total { get; set; }
        public List<InvoiceItemModel> Items { get; set; } = new List<InvoiceItemModel>();
    }

   

}
