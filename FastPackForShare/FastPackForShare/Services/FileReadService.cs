using CsvHelper;
using FastPackForShare.Enums;
using FastPackForShare.Extensions;
using FastPackForShare.Interfaces;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using OfficeOpenXml;
using FastPackForShare.Services.Bases;
using FastPackForShare.Bases;

namespace FastPackForShare.Services;

public sealed class FileReadService<TBaseReportModel> : BaseHandlerService, IFileReadService<TBaseReportModel> where TBaseReportModel : BaseReportModel
{
    public FileReadService(INotificationMessageService iNotificationMessageService) : base(iNotificationMessageService)
    {
    }

    private async Task<MemoryStream> GetMemoryStreamByFile(string path)
    {
        MemoryStream memoryStream = new MemoryStream();

        using (var fileStream = new FileStream(path, FileMode.Open))
        {
            await fileStream.CopyToAsync(memoryStream);
        }

        memoryStream.Position = 0;

        if (File.Exists(path))
            File.Delete(path);

        return memoryStream;
    }

    #region Methods Read EPPLUS

    private IEnumerable<TBaseReportModel> ReadExcelDataEPPLUS(Stream excelFileStream)
    {
        List<TBaseReportModel> list = new List<TBaseReportModel>();
        var arquivoExcel = new ExcelPackage(excelFileStream);
        ExcelWorksheet worksheet = arquivoExcel.Workbook.Worksheets.FirstOrDefault();
        int rows = worksheet.Dimension.Rows;
        int cols = worksheet.Dimension.Columns;

        for (int indexRow = 1; indexRow <= rows; indexRow++)
        {
            for (int indexColumn = 1; indexColumn <= cols; indexColumn++)
            {
                // Adiciona valor no TModel
                //string conteudo = worksheet.Cells[indexRow, indexColumn].Value.ToString();
                // list.Add();
            }
        }

        return list;
    }

    #endregion

    #region Methods Read NPOI

    private IEnumerable<TBaseReportModel> ReadExcelDataNPOI(ISheet sheet)
    {
        List<TBaseReportModel> list = new List<TBaseReportModel>();
        string[] arrColunas = new string[255];
        int arrPos = 0;
        IRow headerRow = sheet.GetRow(0);
        int cellCount = headerRow.LastCellNum;

        for (int j = 0; j < headerRow.LastCellNum; j++)
        {
            NPOI.SS.UserModel.ICell cell = headerRow.GetCell(j);
            if (GuardClauseExtension.IsNull(cell) ||
                GuardClauseExtension.IsNullOrWhiteSpace(cell.ToString())) continue;
            arrColunas[arrPos] = cell.ToString();
            arrPos++;
        }

        for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
        {
            //TModel model = new(); //Entidade
            IRow row = sheet.GetRow(i);
            if (GuardClauseExtension.IsNull(row)) continue;
            if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
            for (int j = row.FirstCellNum; j < cellCount; j++)
            {
                if (GuardClauseExtension.IsNotNull(row.GetCell(j)))
                {
                    //model.XPTO = row.GetCell(j).ToString();
                }

            }
            //list.Add(model);
        }

        return list;
    }

    private ISheet GetExcelStreamNPOI(Stream excelFileStream, string excelExtension)
    {
        if (excelExtension == ".xlsx")
        {
            var xssfWorkBook = new XSSFWorkbook(excelFileStream);
            return xssfWorkBook.GetSheetAt(0);
        }
        else
        {
            var hssfWorkBook = new HSSFWorkbook(excelFileStream);
            return hssfWorkBook.GetSheetAt(0);
        }
    }

    #endregion

    #region Methods Read CSV

    public IEnumerable<TBaseReportModel> ParseCsv<T>(string csvFilePath)
    {
        using var reader = new StreamReader(csvFilePath);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        foreach (var registro in csv.GetRecords<TBaseReportModel>())
            yield return registro;
    }

    #endregion

    public async Task<IEnumerable<TBaseReportModel>> ReadExcelDataFromUploadNPOI(MemoryStream memoryStreamFile)
    {
        ISheet sheet = GetExcelStreamNPOI(memoryStreamFile, $".{EnumFile.Excel.GetDisplayName()}");
        await Task.CompletedTask;
        return ReadExcelDataNPOI(sheet);
    }

    public async Task<IEnumerable<TBaseReportModel>> ReadExcelDataFromFolderNPOI(FileInfo fileInfo)
    {
        if (fileInfo.Extension.Contains(".xlsx") || fileInfo.Extension.Contains(".xls"))
        {
            var stream = File.OpenRead(fileInfo.FullName);
            stream.Position = 0;
            ISheet sheet = GetExcelStreamNPOI(stream, fileInfo.Extension);
            await Task.CompletedTask;
            return ReadExcelDataNPOI(sheet);
        }

        return Enumerable.Empty<TBaseReportModel>();
    }

    public async Task<IEnumerable<TBaseReportModel>> ReadExcelDataFromUploadEPPLUS(MemoryStream memoryStreamFile)
    {
        await Task.CompletedTask;
        return ReadExcelDataEPPLUS(memoryStreamFile);
    }

    public async Task<IEnumerable<TBaseReportModel>> ReadExcelDataFromFolderEPPLUS(FileInfo fileInfo)
    {
        if (fileInfo.Extension.Contains(".xlsx") || fileInfo.Extension.Contains(".xls"))
        {
            var stream = File.OpenRead(fileInfo.FullName);
            stream.Position = 0;
            await Task.CompletedTask;
            return ReadExcelDataEPPLUS(stream);
        }

        return Enumerable.Empty<TBaseReportModel>();
    }

    public async Task<IEnumerable<TBaseReportModel>> ReadCsvData(string csvFilePath)
    {
        var result = ParseCsv<TBaseReportModel>(csvFilePath);
        await Task.CompletedTask;
        return result;
    }

    public async Task<IEnumerable<TBaseReportModel>> ReadCsvDataFromIFormFile(MemoryStream memoryStreamFile)
    {
        var list = Enumerable.Empty<TBaseReportModel>();

        string base64 = Convert.ToBase64String(memoryStreamFile.ToArray());

        using (var mss = new MemoryStream(Convert.FromBase64String(base64.Substring(base64.IndexOf(',') + 1))))
        {
            using (var reader = new StreamReader(memoryStreamFile))
            {
                using (var csvReader = new CsvReader(reader, new CultureInfo("pt-BR")))
                {
                    list = csvReader.GetRecords<TBaseReportModel>();
                }
            }
        }

        return list;
    }

    #region Metodos para realizar Leitura/Escrita de arquivos do tipo Texto

    public async Task<string> ReadFileFromPath(string filePath)
    {
        string result = string.Empty;

        using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            var bytesArray = new byte[fileStream.Length];
            await fileStream.ReadAsync(bytesArray);
            result = Encoding.UTF8.GetString(bytesArray);
        }

        return result;
    }

    /// <summary>
    /// Faz a leitura do arquivo em pedaços de 4kb
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public async Task<string> ReadLargeFileFromPath(string filePath)
    {
        const int bufferSize = 4096;
        var builder = new StringBuilder();
        using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        using (var streamReader = new StreamReader(fileStream))
        {
            char[] buffer = new char[bufferSize];
            int bytesRead;
            while ((bytesRead = streamReader.ReadBlock(buffer, 0, bufferSize)) > 0)
            {
                builder.Append(buffer, 0, bytesRead);
            }
        }

        await Task.CompletedTask;
        return builder.ToString();
    }

    #endregion

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}