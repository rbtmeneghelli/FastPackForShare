﻿using System.ComponentModel;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;
using FastPackForShare.Enums;
using FastPackForShare.Extensions;
using FastPackForShare.Interfaces;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using NPOI.XWPF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using FastPackForShare.Services.Bases;
using FastPackForShare.Bases;
using FastPackForShare.Bases.Generics;

namespace FastPackForShare.Services;

public sealed class FileWriteService<TGenericReportModel> : BaseHandlerService, IFileWriteService<TGenericReportModel> where TGenericReportModel : GenericReportModel
{
    public FileWriteService(INotificationMessageService iNotificationMessageService) : base(iNotificationMessageService)
    {
    }

    #region Methods Write EPPLUS

    private ExcelWorksheet SetWorkSheetDataEPPLUS(ExcelWorksheet workSheet, DataTable dataTable)
    {
        int indexRow = 2;

        foreach (DataRow dataRow in dataTable.Rows)
        {
            int indexColumn = 1;

            foreach (DataColumn col in dataTable.Columns)
            {
                if (dataRow[col.ColumnName] != DBNull.Value)
                {
                    if (bool.TryParse(dataRow[col.ColumnName].ToString(), out _))
                    {
                        workSheet.Cells[indexRow, indexColumn].Value = bool.Parse(dataRow[col.ColumnName].ToString()) ?
                                                                       EnumStatus.Active.GetDisplayShortName() :
                                                                       EnumStatus.Inactive.GetDisplayShortName();
                    }
                    else if (DateTime.TryParse(dataRow[col.ColumnName].ToString(), out _))
                    {
                        workSheet.Cells[indexRow, indexColumn].Value = DateTime.Parse(dataRow[col.ColumnName].ToString()).ToShortDateString();
                    }
                    else if (TimeSpan.TryParse(dataRow[col.ColumnName].ToString(), out _) && dataRow[col.ColumnName].ToString().Length > 10)
                    {
                        if (dataRow[col.ColumnName].ToString().IndexOf("T") != -1)
                            workSheet.Cells[indexRow, indexColumn].Value = TimeSpan.Parse(dataRow[col.ColumnName].ToString()).ToString(@"dd\:hh\:mm");
                    }
                    else if (decimal.TryParse(dataRow[col.ColumnName].ToString(), out _))
                    {
                        workSheet.Cells[indexRow, indexColumn].Value = string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:N}", dataRow[col.ColumnName].ToString());
                    }
                    else
                    {
                        workSheet.Cells[indexRow, indexColumn].Value = dataRow[col.ColumnName].ToString();
                    }
                }
                else
                {
                    workSheet.Cells[indexRow, indexColumn].Value = string.Empty;
                }
                indexColumn++;
            }
            indexRow++;
        }

        return workSheet;
    }

    private ExcelWorksheet SetWorkSheetHeaderEPPLUS(ExcelWorksheet workSheet)
    {
        int countColumn = 1;
        string[] letters = new[] { "A","B","C","D","E","F","G","H","I","J","K","L","M","N",
                               "O","P","Q","R","S","T","U","V","X","Y","Z" };

        var listProperties = SharedExtension.GetDataProperties<TGenericReportModel>();
        foreach (var propertie in listProperties)
        {
            workSheet.Cells[1, countColumn].Value = propertie.DisplayName;
            countColumn++;
        }
        workSheet.Cells[$"A1:{letters[countColumn - 1]}1"].Style.Font.Italic = true;
        return workSheet;
    }

    private ExcelWorksheet GetWorkSheetEPPLUS(ExcelWorksheet workSheet, DataTable dataTable)
    {
        workSheet.TabColor = System.Drawing.Color.Black;
        workSheet.DefaultRowHeight = 12;
        workSheet.Row(1).Height = 20;
        workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        workSheet.Row(1).Style.Font.Bold = true;
        workSheet = SetWorkSheetHeaderEPPLUS(workSheet);
        workSheet = SetWorkSheetDataEPPLUS(workSheet, dataTable);
        return workSheet;
    }

    #endregion

    #region Methods Write NPOI

    private void SetWorkSheetDataNPOI(ISheet excelSheet, DataTable dataTable)
    {
        int indexRow = 1;


        foreach (DataRow dataRow in dataTable.Rows)
        {
            IRow row = excelSheet.CreateRow(indexRow);
            int indexColumn = 0;

            foreach (DataColumn col in dataTable.Columns)
            {
                if (dataRow[col.ColumnName] != DBNull.Value)
                {
                    if (bool.TryParse(dataRow[col.ColumnName].ToString(), out _))
                    {
                        row.CreateCell(indexColumn).SetCellValue(bool.Parse(dataRow[col.ColumnName].ToString()) ?
                                                                 EnumStatus.Active.GetDisplayShortName() :
                                                                 EnumStatus.Inactive.GetDisplayShortName());
                    }
                    else if (DateTime.TryParse(dataRow[col.ColumnName].ToString(), out _))
                    {
                        row.CreateCell(indexColumn).SetCellValue(DateTime.Parse(dataRow[col.ColumnName].ToString()).ToShortDateString());
                    }
                    else if (TimeSpan.TryParse(dataRow[col.ColumnName].ToString(), out _) && dataRow[col.ColumnName].ToString().Length > 10)
                    {
                        if (dataRow[col.ColumnName].ToString().IndexOf("T") != -1)
                            row.CreateCell(indexColumn).SetCellValue(TimeSpan.Parse(dataRow[col.ColumnName].ToString()).ToString(@"dd\:hh\:mm"));
                    }
                    else if (decimal.TryParse(dataRow[col.ColumnName].ToString(), out _))
                    {
                        row.CreateCell(indexColumn).SetCellValue(string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:N}", dataRow[col.ColumnName].ToString()));
                    }
                    else
                    {
                        row.CreateCell(indexColumn).SetCellValue(dataRow[col.ColumnName].ToString());
                    }
                }
                else
                {
                    row.CreateCell(indexColumn).SetCellValue(string.Empty);
                }
                indexColumn++;
            }
            indexRow++;
        }
    }

    private ISheet SetWorkSheetHeaderNPOI(ISheet excelSheet)
    {
        IRow row = excelSheet.CreateRow(0);
        int indexColumn = 0;

        var listProperties = SharedExtension.GetDataProperties<TGenericReportModel>();
        foreach (var propertie in listProperties)
        {
            row.CreateCell(indexColumn).SetCellValue(propertie.DisplayName);
            indexColumn++;
        }

        return excelSheet;
    }

    private IWorkbook GetWorkSheetNPOI(DataTable dataTable)
    {
        IWorkbook workbook = new XSSFWorkbook();
        ISheet excelSheet = workbook.CreateSheet("registros");
        excelSheet = SetWorkSheetHeaderNPOI(excelSheet);
        SetWorkSheetDataNPOI(excelSheet, dataTable);
        return workbook;
    }

    private XWPFDocument SetParagraphWordDocNPOI(XWPFDocument document, ParagraphAlignment paragraphAlignment, string fontFamily, double fontSize, bool isBold, string text, int? indentationFirstLinestring = null)
    {
        var paragraph = document.CreateParagraph();
        paragraph.Alignment = paragraphAlignment;
        paragraph.IndentationFirstLine = indentationFirstLinestring.HasValue ? indentationFirstLinestring.Value : 0;
        XWPFRun run = paragraph.CreateRun();
        run.FontFamily = fontFamily;
        run.FontSize = fontSize;
        run.IsBold = isBold;
        run.SetText(text);
        return document;
    }

    private XWPFDocument GetWordDocNPOI(IEnumerable<string> list)
    {
        XWPFDocument document = new();
        SetParagraphWordDocNPOI(document, ParagraphAlignment.CENTER, "microsoft yahei", 18, true, "TITULO");
        SetParagraphWordDocNPOI(document, ParagraphAlignment.LEFT, "·ÂËÎ", 12, true, string.Join(",", list), 500);
        return document;
    }

    #endregion

    #region Methods Write CSV

    private IEnumerable<string> GetLinesToFillCSV(PropertyInfo[] arrPropertyInfo, string headerOriginalProperty, IEnumerable<TGenericReportModel> list)
    {
        var culture = CultureInfo.GetCultureInfo("pt-BR");
        List<string> lines = new List<string>();

        foreach (var dataObject in list)
        {
            lines.Add(string.Join(";", arrPropertyInfo
                 .Where(a => headerOriginalProperty.Contains(a.Name))
                 .Select(p => p.PropertyType == typeof(decimal) ?
                              string.Format(culture, "{0:C}", p.GetValue(dataObject))
                              : p.PropertyType == typeof(DateTime) || p.PropertyType == typeof(DateTime?) ?
                              string.Format(culture, "{0:dd/MM/yyyy}", p.GetValue(dataObject))
                              : p.GetValue(dataObject))));
        }

        return lines;
    }

    private MemoryStream GetMemoryStreamCsv(string headerDescription, IEnumerable<string> lines)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            TextWriter tw = new StreamWriter(ms, Encoding.UTF8);
            tw.WriteLine(headerDescription);

            foreach (string line in lines)
                tw.WriteLine(line);

            tw.Flush();
            ms.Position = 0;

            return ms;
        }
    }

    #endregion

    public async Task<MemoryStream> CreateExcelFileEPPLUS(IEnumerable<TGenericReportModel> list, string excelName)
    {
        ExcelPackage excelPackage = new();
        string path = Path.Combine(Directory.GetCurrentDirectory(), excelName);
        var dataTable = SharedExtension.ConvertToDataTable(list);
        var workSheet = excelPackage.Workbook.Worksheets.Add("registros");
        workSheet = GetWorkSheetEPPLUS(workSheet, dataTable);

        for (int i = 1; i <= dataTable.Columns.Count; i++)
        {
            workSheet.Column(i).AutoFit();
        }

        FileStream fileStream = File.Create(path);
        fileStream.Close();
        File.WriteAllBytes(path, excelPackage.GetAsByteArray());
        excelPackage.Dispose();

        return await SharedExtension.GetMemoryStreamByFile(path);
    }

    public async Task<MemoryStream> CreateExcelFileNPOI(IEnumerable<TGenericReportModel> list, string excelName)
    {
        string path = Path.Combine(Directory.GetCurrentDirectory(), excelName);
        var dataTable = SharedExtension.ConvertToDataTable(list);

        using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
        {
            var workBook = GetWorkSheetNPOI(dataTable);
            workBook.Write(fs);
        }

        return await SharedExtension.GetMemoryStreamByFile(path);
    }

    public async Task<MemoryStream> CreateWordFile(IEnumerable<string> list, string wordName)
    {
        string path = Path.Combine(Directory.GetCurrentDirectory(), wordName);

        using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
        {
            XWPFDocument doc = GetWordDocNPOI(list);
            doc.Write(fs);

            return await SharedExtension.GetMemoryStreamByFile(path);
        }
    }

    public async Task<MemoryStream> CreateCsvFile(IEnumerable<TGenericReportModel> list)
    {
        Type item = typeof(TGenericReportModel);
        var arrPropertyInfo = item.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        arrPropertyInfo = arrPropertyInfo.OrderBy(p =>
        {
            var order = (Attribute.GetCustomAttribute(p, typeof(DataMemberAttribute)) as DataMemberAttribute)?.Order;
            return order ?? 0;

        }).ToArray();

        var headerWithDescription = arrPropertyInfo.Where(p => Attribute.IsDefined(p, typeof(DescriptionAttribute))
            && !string.IsNullOrWhiteSpace((Attribute.GetCustomAttribute(p, typeof(DescriptionAttribute)) as DescriptionAttribute).Description));
        var headerOriginalProperty = string.Join(";", headerWithDescription.Select(p => p.Name));
        var headerDescription = string.Join(";", headerWithDescription
            .Select(p => (Attribute.GetCustomAttribute(p, typeof(DescriptionAttribute)) as DescriptionAttribute).Description));

        IEnumerable<string> lines = GetLinesToFillCSV(arrPropertyInfo, headerOriginalProperty, list);
        MemoryStream memoryStreamCsv = await Task.FromResult(GetMemoryStreamCsv(headerDescription, lines));

        return memoryStreamCsv;
    }

    #region Metodos para realizar Leitura/Escrita de arquivos do tipo Texto

    public async Task CreateAndWriteFileToPath(string filePath, string content)
    {
        await File.WriteAllTextAsync(filePath, content);
    }

    /// <summary>
    /// Faz a escrita do arquivo em pedaços de 1024 bytes
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public async Task CreateAndWriteLargeFileToPath(string filePath, string content)
    {
        using (FileStream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        {
            byte[] buffer = Encoding.UTF8.GetBytes(content);
            int chunkSize = 1024;
            for (int i = 0; i < buffer.Length; i += chunkSize)
            {
                int remainingBytes = buffer.Length - i;
                int bytesToWrite = remainingBytes < chunkSize ? remainingBytes : chunkSize;
                stream.Write(buffer, i, bytesToWrite);
            }

            await Task.CompletedTask;
        }
    }

    #endregion

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
