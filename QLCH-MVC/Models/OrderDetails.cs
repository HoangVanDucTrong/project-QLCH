namespace QLCH_MVC.Models
{
    public class OrderDetails
    {
        public int BanId { get; set; }
        public int CTDHId { get; set; }
        public DateTime NgayTao { get; set; }
        public string ImageCheckBank { get; set; }
        public int StoreId { get; set; }
        public List<Product> Products { get; set; }
    }

    public class Product
    {
        public string Ten { get; set; }
        public int SoLuong { get; set; }
        public string ImageBase64 { get; set; }
        public decimal TongTien { get; set; }
    }

}
