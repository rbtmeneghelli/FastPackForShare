using SkiaSharp;
using ZXing.SkiaSharp;
using BarcodeReader = ZXing.SkiaSharp.BarcodeReader;
using FastPackForShare.Extensions;
using Microsoft.AspNetCore.Http;
using FastPackForShare.Interfaces;
using FastPackForShare.Models;

namespace FastPackForShare.Services;

public class QRCodeService : IQRCodeService
{
    public string ReadQrCode(IFormFile file)
    {
        using var stream = file.OpenReadStream();
        SKBitmap bitmap = SKBitmap.Decode(stream);
        BarcodeReader reader = new BarcodeReader
        {
            AutoRotate = true,
        };

        var result = reader.Decode(bitmap);
        if (GuardClauseExtension.IsNotNull(result))
        {
            return result.Text;
        }

        return string.Empty;
    }

    public byte[] CreateQRCode(QRCodeFileModel qRCodeFileModel)
    {
        BarcodeWriter writer = new BarcodeWriter
        {
        };

        SKBitmap bitmap = writer.Write(qRCodeFileModel.Content);

        byte[] imageBytes;
        using (SKImage image = SKImage.FromBitmap(bitmap))
        using (SKData encodedData = image.Encode(SKEncodedImageFormat.Png, 100))
        {
            imageBytes = encodedData.ToArray();
        }

        return imageBytes;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
