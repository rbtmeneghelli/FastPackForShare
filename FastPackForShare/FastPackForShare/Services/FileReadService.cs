using CsvHelper;
using FastPackForShare.Enums;
using FastPackForShare.Extensions;
using FastPackForShare.Interfaces;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using OfficeOpenXml;
using FastPackForShare.Services.Bases;
using FastPackForShare.Bases.Generics;
using System.Data;
using System.Reflection;
using DocumentFormat.OpenXml.Spreadsheet;
using FastPackForShare.Models;

namespace FastPackForShare.Services;

public sealed class FileReadService<TGenericReportModel> : BaseHandlerService, IFileReadService<TGenericReportModel> where TGenericReportModel : GenericReportModel
{
    public FileReadService(INotificationMessageService iNotificationMessageService) : base(iNotificationMessageService)
    {
    }

    #region Methods Read EPPLUS

    private IEnumerable<TGenericReportModel> ReadExcelDataEPPLUS(Stream excelFileStream)
    {
        List<TGenericReportModel> list = new List<TGenericReportModel>();
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

    private IEnumerable<TGenericReportModel> ReadExcelDataNPOI(ISheet sheet)
    {
        List<TGenericReportModel> list = new List<TGenericReportModel>();
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
            if (row.Cells.All(d => d.CellType == NPOI.SS.UserModel.CellType.Blank)) continue;
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

    public async Task<IEnumerable<TGenericReportModel>> ReadCsvFileFromPath(string csvFilePath)
    {
        var result = ParseCsv<TGenericReportModel>(csvFilePath);
        await Task.CompletedTask;
        return result;
    }

    public async Task<List<string[]>> ReadCsvFileFromStream(Stream stream)
    {
        var result = new List<string[]>();

        using (var reader = new StreamReader(stream, Encoding.UTF8))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            await foreach (var record in csv.GetRecordsAsync<TGenericReportModel>())
            {
                var values = ((IDictionary<string, object>)record).Values.Select(v => v?.ToString() ?? "").ToArray();
                result.Add(values);

                if (result.Count >= 100000)
                    break;
            }
        }

        return result;
    }

    public IEnumerable<TGenericReportModel> ParseCsv<T>(string csvFilePath)
    {
        using var reader = new StreamReader(csvFilePath);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        foreach (var registro in csv.GetRecords<TGenericReportModel>())
            yield return registro;
    }

    #endregion

    public async Task<IEnumerable<TGenericReportModel>> ReadExcelDataFromUploadNPOI(MemoryStream memoryStreamFile)
    {
        ISheet sheet = GetExcelStreamNPOI(memoryStreamFile, $".{EnumFile.Excel.GetDisplayName()}");
        await Task.CompletedTask;
        return ReadExcelDataNPOI(sheet);
    }

    public async Task<IEnumerable<TGenericReportModel>> ReadExcelDataFromFolderNPOI(FileInfo fileInfo)
    {
        if (fileInfo.Extension.Contains(".xlsx") || fileInfo.Extension.Contains(".xls"))
        {
            var stream = File.OpenRead(fileInfo.FullName);
            stream.Position = 0;
            ISheet sheet = GetExcelStreamNPOI(stream, fileInfo.Extension);
            await Task.CompletedTask;
            return ReadExcelDataNPOI(sheet);
        }

        return Enumerable.Empty<TGenericReportModel>();
    }

    public async Task<IEnumerable<TGenericReportModel>> ReadExcelDataFromUploadEPPLUS(MemoryStream memoryStreamFile)
    {
        await Task.CompletedTask;
        return ReadExcelDataEPPLUS(memoryStreamFile);
    }

    public async Task<IEnumerable<TGenericReportModel>> ReadExcelDataFromFolderEPPLUS(FileInfo fileInfo)
    {
        if (fileInfo.Extension.Contains(".xlsx") || fileInfo.Extension.Contains(".xls"))
        {
            var stream = File.OpenRead(fileInfo.FullName);
            stream.Position = 0;
            await Task.CompletedTask;
            return ReadExcelDataEPPLUS(stream);
        }

        return Enumerable.Empty<TGenericReportModel>();
    }

    public async Task<IEnumerable<TGenericReportModel>> ReadCsvFileFromIFormFile(MemoryStream memoryStreamFile)
    {
        var list = Enumerable.Empty<TGenericReportModel>();

        string base64 = Convert.ToBase64String(memoryStreamFile.ToArray());

        using (var mss = new MemoryStream(Convert.FromBase64String(base64.Substring(base64.IndexOf(',') + 1))))
        {
            using (var reader = new StreamReader(memoryStreamFile))
            {
                using (var csvReader = new CsvReader(reader, new CultureInfo("pt-BR")))
                {
                    list = csvReader.GetRecords<TGenericReportModel>();
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

    public IEnumerable<TGenericReportModel> ReadCustomFileFromPath(string filePath, char separator = ',')
    {
        DataTable dataTable = new();
        string line = string.Empty;
        ListExtensionMethod listExtensionMethod = new();

        if (!File.Exists(filePath))
            throw new FileNotFoundException("O arquivo solicitado não existe!", filePath);

        using var reader = new StreamReader(filePath);

        PropertyInfo[] properties = typeof(TGenericReportModel).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var prop in properties)
        {
            dataTable.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
        }

        while ((line = reader.ReadLine()) != null)
        {
            if (GuardClauseExtension.IsNullOrWhiteSpace(line)) continue;

            ReadOnlySpan<char> data = line.AsSpan();
            data = data.StartsWith([separator]) ? data.Slice(1) : data;
            data = data.EndsWith([separator]) ? data.Slice(0, data.Length - 1) : data;

            var values = new object[properties.Length];

            for (int i = 0; i < properties.Length; i++)
            {
                int positionPipe = data.IndexOf(separator);

                if (positionPipe == -1 || i == properties.Length - 1)
                {
                    values[i] = GetDataFromFile(data);
                    break;
                }

                values[i] = GetDataFromFile(data.Slice(0, positionPipe));
                data = data.Slice(positionPipe + 1);
            }

            dataTable.Rows.Add(values);
        }

        return dataTable.Rows.Count > 0 ?
               listExtensionMethod.ConvertToList<TGenericReportModel>(dataTable)
               : Enumerable.Empty<TGenericReportModel>();
    }

    private object GetDataFromFile(ReadOnlySpan<char> data)
    {
        if (bool.TryParse(data, out _))
            return bool.Parse(data);
        else if (DateTime.TryParse(data, out _))
            return DateTime.Parse(data).ToShortDateString();
        else if (TimeSpan.TryParse(data, out _) && data.Length > 10)
        {
            if (data.IndexOf("T") != -1) { return TimeSpan.Parse(data).ToString(@"dd\:hh\:mm"); }
        }
        else if (decimal.TryParse(data, out _))
            return decimal.Parse(data);
        else if (int.TryParse(data, out _))
            return int.Parse(data);

        return data.ToString();
    }

    #endregion

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}