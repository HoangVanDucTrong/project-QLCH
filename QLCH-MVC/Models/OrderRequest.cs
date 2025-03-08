namespace QLCH_MVC.Models
{
    public class OrderRequest
    {
        public int StoreId { get; set; }
        public int BanId { get; set; }
        public decimal TongTien { get; set; }
        public string ImageCheckBank { get; set; } // Ảnh hóa đơn lưu dưới dạng Base64
        public List<ProductOrder> Products { get; set; }
    }

    public class ProductOrder
    {
        public int SanPhamId { get; set; }
        public string Tensp { get; set; } // Thêm tên sản phẩm để dễ hiển thị
        public int SoLuong { get; set; }
        public decimal Gia { get; set; }
        public string? GhiChu { get; set; } // Ghi chú của khách hàng (nếu có)
    }

}
