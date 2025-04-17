using FastPackForShare.Bases;

namespace FastPackForShare.Interfaces;

public interface IFileReadService<TBaseReportModel> : IDisposable where TBaseReportModel : BaseReportModel
{
    Task<IEnumerable<TBaseReportModel>> ReadExcelDataFromUploadNPOI(MemoryStream memoryStreamFile);
    Task<IEnumerable<TBaseReportModel>> ReadExcelDataFromFolderNPOI(FileInfo fileInfo);
    Task<IEnumerable<TBaseReportModel>> ReadExcelDataFromUploadEPPLUS(MemoryStream memoryStreamFile);
    Task<IEnumerable<TBaseReportModel>> ReadExcelDataFromFolderEPPLUS(FileInfo fileInfo);
    Task<IEnumerable<TBaseReportModel>> ReadCsvData(string csvFilePath);
    Task<IEnumerable<TBaseReportModel>> ReadCsvDataFromIFormFile(MemoryStream memoryStreamFile);
    Task<string> ReadFileFromPath(string filePath);
    Task<string> ReadLargeFileFromPath(string filePath);
}
