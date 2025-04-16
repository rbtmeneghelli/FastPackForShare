namespace FastPackForShare.Models;

public sealed class CustomResponseFileModel
{
    public byte[] FileMemoryStream { get; init; }
    public string Type { get; init; }
    public string Extension { get; init; }

    public CustomResponseFileModel(byte[] fileMemoryStream, string type, string extension) =>
    (FileMemoryStream, Type, Extension) = (fileMemoryStream, type, extension);
}
