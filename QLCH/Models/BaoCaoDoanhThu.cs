using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class BaoCaoDoanhThu
{
    [Key]
    public int ReportId { get; set; }

    [Required]
    public int StoreId { get; set; }

    [Required]
    [Column(TypeName = "date")]
    public DateTime? NgayBaoCao { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal TongDoanhThu { get; set; }

    [Required]
    public int SoLuongBanRa { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? DTTheoThang { get; set; } // Cho phép null


    public DateTime? Thang { get; set; } // Cho phép null

    public int? TransactionId { get; set; }

}
