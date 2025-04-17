namespace FastPackForShare.Models;

public record QRCodeFileModel
{
    public int Width { get; set; }
    public int Height { get; set; }
    public string Content { get; set; }
}
