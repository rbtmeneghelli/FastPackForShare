using FastPackForShare.Enums;
using FastPackForShare.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastPackForShare.Helpers;

public static class HelperFile
{
    private static string ValidateContentTypeFile(string key)
    {
        Dictionary<string, string> dictionary = new()
    {
        { ".jpg", "image/jpeg" },
        { ".jpeg", "image/jpeg" },
        { ".png", "image/png" },
        { ".pdf", "application/pdf" },
        { ".mp4", "video/mp4" }
    };

        if (dictionary.TryGetValue(key, out var value))
            return value;

        return string.Empty;
    }

    public static void Writelog(string diretorio, DateTime pData, TimeSpan pHora, string pClasse, string pMetodo, string pErro)
    {
        var caminhoArquivo = Path.GetTempFileName();
        string arqLog = $"{diretorio}\\Log_{GuidExtension.GetGuidDigits("N")}.txt";

        if (!Directory.Exists(diretorio))
            Directory.CreateDirectory(diretorio);

        if (File.Exists(arqLog))
        {
            using (StreamWriter sw = File.AppendText(arqLog))
            {
                var texto = string.Format("Data:{0:d} Hora:{1} Classe:{2} Metodo:{3} Erro:{4}", pData, pHora.ToString(@"hh\:mm"), pClasse, pMetodo, pErro);
                sw.WriteLine(texto);
            }
        }

        else
        {
            using (StreamWriter sw = File.AppendText(arqLog))
            {
                var texto = string.Format("Data:{0:d} Hora:{1} Classe:{2} Metodo:{3} Erro:{4}", pData, pHora.ToString(@"hh\:mm"), pClasse, pMetodo, pErro);
                sw.WriteLine(texto);
            }
        }
    }

    public static bool ValidateFile(IFormFile formFile, string[] arrExtensions, double maxSizeFile)
    {
        var extension = Path.GetExtension(formFile.FileName).ToLowerInvariant();
        var contentType = ValidateContentTypeFile(extension);

        if (!arrExtensions.Contains(extension))
            return false;

        if (!formFile.ContentType.Equals(contentType))
            return false;

        if (formFile.Length > CalculateMaxSizeFile(maxSizeFile, EnumFileSize.MB))
            return false;

        return true;
    }

    public static bool ExistFile(IFormFile formFile)
    {
        if (formFile == null || formFile.Length == 0)
            return false;

        return true;
    }

    public static double CalculateMaxSizeFile(double size, EnumFileSize enumFileSize)
    {

        double maxSize = enumFileSize switch
        {
            EnumFileSize.KB => size * 1024,
            EnumFileSize.MB => size * 1024 * 1024,
            EnumFileSize.GB => size * 1024 * 1024 * 1024,
            EnumFileSize.TB => size * 1024 * 1024 * 1024 * 1024,
            _ => size
        };

        return maxSize;
    }

    public static string GetBase64FromStream(Stream streamFile)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            streamFile.CopyTo(ms);
            return Convert.ToBase64String(ms.ToArray());
        }
    }

    public static async Task<MemoryStream> GetMemoryStreamByFile(string path)
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

    public static async Task<byte[]> SetFileToByteArray(IFormFile formfile)
    {
        MemoryStream ms = new MemoryStream(new byte[formfile.Length]);
        await formfile.CopyToAsync(ms);
        return ms.ToArray();
    }

    public static void CleanFiles(string rootFolder, string user)
    {
        string[] arrFiles = new string[] { "Arquivo1", "Arquivo2" };
        foreach (var item in arrFiles)
        {
            string fileName = $"{item}_{user}.{EnumFile.Excel.GetDisplayName()}";
            FileInfo fullPath = new FileInfo(Path.Combine(rootFolder, fileName));
            if (fullPath.Exists)
                fullPath.Delete();
        }
    }

    #region Metodos auxiliares para diminuir o tamanho de arquivos muito extensos, para arquivos compactos

    public static byte[] CompressStream(byte[] originalSource)
    {
        using (var outStream = new MemoryStream())
        {
            using (var gzip = new GZipStream(outStream, CompressionMode.Compress))
            {
                gzip.Write(originalSource, 0, originalSource.Length);
            }
            return outStream.ToArray();
        }
    }

    public static byte[] DecompressStream(byte[] originalSource)
    {
        using (var sourceStream = new MemoryStream(originalSource))
        {
            using (var outStream = new MemoryStream())
            {
                using (var gzip = new GZipStream(sourceStream, CompressionMode.Decompress))
                {
                    gzip.CopyTo(outStream);
                }
                return outStream.ToArray();
            }
        }
    }

    public static string GetMemoryStreamExtension(EnumFile key)
    {
        Dictionary<EnumFile, string> dictionary = new Dictionary<EnumFile, string>
    {
        { EnumFile.Excel, $"{EnumFile.Excel.GetDisplayName()}" },
        { EnumFile.Pdf, $"{EnumFile.Pdf.GetDisplayName()}" },
        { EnumFile.Word, $"{EnumFile.Word.GetDisplayName()}" }
    };
        return dictionary[key];
    }

    public static (string Type, string Extension) GetMemoryStreamType(EnumFile key)
    {
        Dictionary<EnumFile, (string, string)> dictionary = new Dictionary<EnumFile, (string, string)>
    {
        { EnumFile.Excel, ("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "xlsx") },
        { EnumFile.Pdf, ("application/pdf", "pdf") },
        { EnumFile.Word, ("application/octet-stream", "docx") },
        { EnumFile.Zip, ("application/zip", "zip") },
        { EnumFile.Png, ("image/png", "png") }
    };

        return dictionary[key];
    }

    #endregion
}
