using FastPackForShare.Bases.Generics;

namespace FastPackForShare.Interfaces;

public interface IFileWriteService<TGenericReportModel> : IDisposable where TGenericReportModel : GenericReportModel
{
    Task<MemoryStream> CreateExcelFileEPPLUS(IEnumerable<TGenericReportModel> list, string excelName);
    Task<MemoryStream> CreateExcelFileNPOI(IEnumerable<TGenericReportModel> list, string excelName);
    Task<MemoryStream> CreateCsvFile(IEnumerable<TGenericReportModel> list);
    Task<MemoryStream> CreateExcelFile(IEnumerable<TGenericReportModel> list, string excelName, string sheetName);
    Task<MemoryStream> CreateWordFileNPOI(IEnumerable<string> list, string wordName);
    Task<MemoryStream> CreateWordFile(IEnumerable<string> list, string wordName);
    Task CreateAndWriteTextFileToPath(string filePath, string content);
    Task CreateAndWriteTextLargeFileToPath(string filePath, string content);
    Task<MemoryStream> CreatePdfFile(List<string> list, string pdfHeader, string pdfName);
}
