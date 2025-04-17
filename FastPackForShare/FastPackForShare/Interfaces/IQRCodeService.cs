using FastPackForShare.Models;
using Microsoft.AspNetCore.Http;

namespace FastPackForShare.Interfaces;

public interface IQRCodeService : IDisposable
{
    byte[] CreateQRCode(QRCodeFileModel qRCodeFileModel);
    string ReadQrCode(IFormFile file);
}
