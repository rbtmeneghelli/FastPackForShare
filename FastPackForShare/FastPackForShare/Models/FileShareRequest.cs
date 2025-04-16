namespace FastPackForShare.Models;

public sealed record FileShareRequest
{
    public string ConnectionString { get; set; }
    public string FileName { get; set; }
    public string FileExtension { get; set; }
    public string ShareReference { get; set; }
}   
