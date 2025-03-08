namespace QLCH.Models
{
    public class OrderRequest
    {
        public int StoreId { get; set; }
        public int BanId { get; set; }
        public string ImageCheckBank { get; set; } // Ảnh hóa đơn lưu dưới dạng Base64
        public List<ProductOrder> Products { get; set; }
    }

    public class ProductOrder
    {
        public int SanPhamId { get; set; }
        public int SoLuong { get; set; }
        public decimal Gia { get; set; }
          public decimal TongTien { get; set; }
    }

}
