using FastPackForShare.Helpers;

namespace FastPackForShare.Models;

public record FileShareResponse
{
    public bool IsSuccess { get; set; } = false;
    public string FileName { get; set; } = string.Empty;
    public string FileExtension { get; set; } = string.Empty;
    public string DataBase64 { get; set; } = string.Empty;

    public FileShareResponse()
    {

    }

    public FileShareResponse(string fileName, string fileExtension, Stream streamFile)
    {
        if (streamFile != null)
        {
            IsSuccess = true;
            FileName = fileName;
            FileExtension = fileExtension;
            DataBase64 = HelperFile.GetBase64FromStream(streamFile);
        }
    }
}
