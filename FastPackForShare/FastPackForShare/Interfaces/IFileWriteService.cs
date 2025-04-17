using FastPackForShare.Bases;

namespace FastPackForShare.Interfaces;

public interface IFileWriteService<TBaseReportModel> : IDisposable where TBaseReportModel : BaseReportModel
{
    Task<MemoryStream> CreateExcelFileEPPLUS(IEnumerable<TBaseReportModel> list, string excelName);
    Task<MemoryStream> CreateExcelFileNPOI(IEnumerable<TBaseReportModel> list, string excelName);
    Task<MemoryStream> CreateWordFile(IEnumerable<string> list, string wordName);
    Task<MemoryStream> CreateCsvFile(IEnumerable<TBaseReportModel> list);
    Task CreateAndWriteFileToPath(string filePath, string content);
    Task CreateAndWriteLargeFileToPath(string filePath, string content);
}
