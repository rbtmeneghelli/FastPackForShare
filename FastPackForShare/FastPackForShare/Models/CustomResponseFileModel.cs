namespace FastPackForShare.Models;

public sealed class CustomResponseFileModel
{
    public required byte[] FileMemoryStream { get; init; }
    public required string Type { get; init; }
    public required string Extension { get; init; }

    public CustomResponseFileModel(byte[] fileMemoryStream, string type, string extension) =>
    (FileMemoryStream, Type, Extension) = (fileMemoryStream, type, extension);
}
