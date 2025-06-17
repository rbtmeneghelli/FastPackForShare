using FastPackForShare.Bases.Generics;

namespace FastPackForShare.Interfaces;

public interface IFileReadService<TGenericReportModel> : IDisposable where TGenericReportModel : GenericReportModel
{
    Task<IEnumerable<TGenericReportModel>> ReadExcelDataFromUploadNPOI(MemoryStream memoryStreamFile);
    Task<IEnumerable<TGenericReportModel>> ReadExcelDataFromFolderNPOI(FileInfo fileInfo);
    Task<IEnumerable<TGenericReportModel>> ReadExcelDataFromUploadEPPLUS(MemoryStream memoryStreamFile);
    Task<IEnumerable<TGenericReportModel>> ReadExcelDataFromFolderEPPLUS(FileInfo fileInfo);
    Task<IEnumerable<TGenericReportModel>> ReadCsvData(string csvFilePath);
    Task<IEnumerable<TGenericReportModel>> ReadCsvDataFromIFormFile(MemoryStream memoryStreamFile);
    Task<string> ReadFileFromPath(string filePath);
    Task<string> ReadLargeFileFromPath(string filePath);
    IEnumerable<TGenericReportModel> ReadCustomFileFromPath(string filePath, char separator = ',');
}
