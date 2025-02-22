<<<<<<< HEAD
﻿using Microsoft.AspNetCore.Mvc;
using QLCH.Data;
using QLCH.Models;
using QRCoder;
using System.Drawing;
using System.IO;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class QRController : ControllerBase
{
  
    private readonly QLCHDbConText _context;

    public QRController(QLCHDbConText context)
    {
        _context = context;
    }

    [HttpPost]
    public IActionResult CreateQR(int banId)
    {
        var ban = _context.bans.Find(banId);

        if (ban == null)
        {
            return NotFound("Ban không tồn tại");
        }

        /*Tạo dữ liệu QR từ BanId*/
        string qrData = $"BanId={ban.BanId}&StoreId={ban.StoreId}";
        /*
        string qrData = $"http://192.168.1.18:7126/store/products?banId={ban.BanId}&storeId={ban.StoreId}";*/


        // Lưu dữ liệu vào cột DuLieuMaQR trong bảng QRs
        var qr = new QR
        {
            BanId = ban.BanId,
            DuLieuMaQR = qrData
        };
            
        _context.QRs.Add(qr);
        _context.SaveChanges();
        // Tạo mã QR
        using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
        {
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrData, QRCodeGenerator.ECCLevel.Q);
            using (QRCode qrCode = new QRCode(qrCodeData))
            {
                using (Bitmap qrCodeImage = qrCode.GetGraphic(20))
                {
                    using (var ms = new MemoryStream())
                    {
                        qrCodeImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        return File(ms.ToArray(), "image/png");
                    }
                }
            }
        }
    }
    [HttpGet("store/products")]
    public IActionResult GetProducts(int banId, int storeId)
    {
        var products = _context.SanPhams
                               .Where(p => p.StoreId == storeId)
                               .ToList();

        if (products == null || !products.Any())
        {
            return NotFound("Không có sản phẩm nào được tìm thấy cho cửa hàng này.");
        }

        return Ok(products);
    }

}
=======
﻿using Microsoft.AspNetCore.Mvc;
using QLCH.Data;
using QLCH.Models;
using QRCoder;
using System.Drawing;
using System.IO;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class QRController : ControllerBase
{
  
    private readonly QLCHDbConText _context;

    public QRController(QLCHDbConText context)
    {
        _context = context;
    }

    [HttpPost]
    public IActionResult CreateQR(int banId)
    {
        var ban = _context.bans.Find(banId);

        if (ban == null)
        {
            return NotFound("Ban không tồn tại");
        }

        /*Tạo dữ liệu QR từ BanId*/
        string qrData = $"BanId={ban.BanId}&StoreId={ban.StoreId}";
        /*
        string qrData = $"http://192.168.1.18:7126/store/products?banId={ban.BanId}&storeId={ban.StoreId}";*/


        // Lưu dữ liệu vào cột DuLieuMaQR trong bảng QRs
        var qr = new QR
        {
            BanId = ban.BanId,
            DuLieuMaQR = qrData
        };
            
        _context.QRs.Add(qr);
        _context.SaveChanges();
        // Tạo mã QR
        using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
        {
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrData, QRCodeGenerator.ECCLevel.Q);
            using (QRCode qrCode = new QRCode(qrCodeData))
            {
                using (Bitmap qrCodeImage = qrCode.GetGraphic(20))
                {
                    using (var ms = new MemoryStream())
                    {
                        qrCodeImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        return File(ms.ToArray(), "image/png");
                    }
                }
            }
        }
    }
    [HttpGet("store/products")]
    public IActionResult GetProducts(int banId, int storeId)
    {
        var products = _context.SanPhams
                               .Where(p => p.StoreId == storeId)
                               .ToList();

        if (products == null || !products.Any())
        {
            return NotFound("Không có sản phẩm nào được tìm thấy cho cửa hàng này.");
        }

        return Ok(products);
    }

}
>>>>>>> 2cd039424233f099f062a952f82ef6ddcda03b12
