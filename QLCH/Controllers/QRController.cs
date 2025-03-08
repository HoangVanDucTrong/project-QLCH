
﻿using Microsoft.AspNetCore.Mvc;
using QLCH.Data;
using QLCH.Models;
using QRCoder;
using System.Drawing;
using System.IO;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using QLCH.Models.IRepository;
[ApiController]
[Route("api/[controller]")]

public class QRController : ControllerBase
{
  
    private readonly QLCHDbConText _context;
    private readonly IGetClaimsFromToken _getClaimsFromToken;
    public QRController(QLCHDbConText context, IGetClaimsFromToken getClaimsFromToken)
    {
        _context = context;
        _getClaimsFromToken = getClaimsFromToken;
    }

    [HttpGet("CreateQR")]
    [Authorize]
    public IActionResult CreateQR(int banId)
    {
      

        if (banId <= 0)
        {
            return BadRequest(new { error = "ID bàn không hợp lệ" });
        }
        var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        var claimsPrincipal = _getClaimsFromToken.laytoken(token);

        var storeId = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "StoreId")?.Value;

        var ban = _context.bans.Find(banId);

        if (ban == null)
        {
            return NotFound("Bàn không tồn tại");
        }
       ban.StoreId=int.Parse(storeId);
        // Tạo dữ liệu QR
        string qrData = $"https://f870-2405-4803-b1ba-600-20e6-783e-b0ab-222c.ngrok-free.app/Ban/ShowProducts?banid={banId}&storeid={storeId}";


        // Kiểm tra nếu QR đã tồn tại trong DB
        var existingQR = _context.QRs.FirstOrDefault(q => q.BanId == ban.BanId);
        if (existingQR == null)
        {
            var qr = new QR { BanId = ban.BanId, DuLieuMaQR = qrData };
            _context.QRs.Add(qr);
            _context.SaveChanges();
        }

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
        Console.WriteLine($"🔍 Nhận request: storeId={storeId}, banId={banId}");
        // Kiểm tra xem bàn có hợp lệ không
        var ban = _context.bans.FirstOrDefault(b => b.BanId == banId && b.StoreId == storeId);
        if (ban == null)
        {
            return NotFound("Bàn không tồn tại trong cửa hàng này.");
        }

        // Lấy danh sách sản phẩm theo storeId
        var products = _context.SanPhams
                               .Where(p => p.StoreId == storeId)
                               .ToList();

        if (products == null || !products.Any())
        {
            return NotFound("Không có sản phẩm nào được tìm thấy cho cửa hàng này.");
        }

        // Trả về danh sách sản phẩm
        return Ok(products);
    }


}
