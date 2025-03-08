namespace QLCH_MVC.Models
{
    public class SanPhamBanViewModel
    {
        public IEnumerable<SanPham> SanPhams { get; set; }
        public IEnumerable<Ban> Bans { get; set; }
    }
    public class BanViewModel
    {

        public IEnumerable<Ban> Bans { get; set; }
    }
    public class SanPhamViewModel
    {

        public IEnumerable<SanPham> sanPhams { get; set; }
    }
}
