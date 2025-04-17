using FastPackForShare.Bases.Generics;

namespace FastPackForShare.Interfaces;

public interface IFileWriteService<TGenericReportModel> : IDisposable where TGenericReportModel : GenericReportModel
{
    Task<MemoryStream> CreateExcelFileEPPLUS(IEnumerable<TGenericReportModel> list, string excelName);
    Task<MemoryStream> CreateExcelFileNPOI(IEnumerable<TGenericReportModel> list, string excelName);
    Task<MemoryStream> CreateWordFile(IEnumerable<string> list, string wordName);
    Task<MemoryStream> CreateCsvFile(IEnumerable<TGenericReportModel> list);
    Task CreateAndWriteFileToPath(string filePath, string content);
    Task CreateAndWriteLargeFileToPath(string filePath, string content);
}
