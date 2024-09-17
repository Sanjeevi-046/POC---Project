namespace POC.CommonModel.Models
{
    public class CommonProductQuantityModel
    {
        public CommonProductModel? ProductList { get; set; }
        public int Quantity { get; set; } = 1;
        public int UserID { get; set; }
        public int ProductID { get; set; }

    }
}
