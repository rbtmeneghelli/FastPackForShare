using FastPackForShare.Models;

namespace FastPackForShare.Helpers;

public static class HelperDriver
{
    public static void ShowAll()
    {
        DriveInfo[] drives = DriveInfo.GetDrives();
        Array.ForEach(drives, drive =>
        {
            if (drive.IsReady)
            {
                Console.WriteLine($"Drive {drive.Name} esta pronto.");
                Console.WriteLine($"Espaço disponível livre: {drive.AvailableFreeSpace} " +
                 $"bytes ou {FormatBytes(drive.AvailableFreeSpace)}");
                Console.WriteLine($"Formato : {drive.DriveFormat}");
                Console.WriteLine($"Tipo: {drive.DriveType}");
                Console.WriteLine($"Nome: {drive.Name}");
                Console.WriteLine("Nome Completo da Raiz : " + $"{drive.RootDirectory.FullName}");
                Console.WriteLine($"Espaço total Livre : {drive.TotalFreeSpace} bytes ou {FormatBytes(drive.TotalFreeSpace)}");
                Console.WriteLine($"Espaço Total : {drive.TotalSize} bytes ou {FormatBytes(drive.TotalSize)}");
                Console.WriteLine($"Volume Label: {drive.VolumeLabel}");
            }
            else
            {
                Console.WriteLine($"Drive {drive.Name} não esta pronto.");
            }
            Console.WriteLine();
        });
    }

    public static void ShowAllAlternative()
    {
        DriveInfo[] drives = DriveInfo.GetDrives();
        foreach (DriveInfo drive in drives)
        {
            if (drive.IsReady)
            {
                Console.WriteLine($"Nome do Drive: {drive.Name}");
                Console.WriteLine($"Formato: {drive.DriveFormat}");
                Console.WriteLine($"Tipo: {drive.DriveType}");
                Console.WriteLine($"Diretório Raiz : {drive.RootDirectory}");
                Console.WriteLine($"Volume label: {drive.VolumeLabel}");
                Console.WriteLine($"Espaço Livre: {drive.TotalFreeSpace}");
                Console.WriteLine($"Espaço disponível: {drive.AvailableFreeSpace}");
                Console.WriteLine($"Tamanho Total: {drive.TotalSize}");
                Console.WriteLine();
                Console.ReadLine();
            }
        }
    }

    public static IEnumerable<DropDownListModel> GetListDrivers()
    {
        int count = 0;
        foreach (var drive in DriveInfo.GetDrives())
        {
            if (drive.TotalFreeSpace / drive.TotalSize * 100 < 90)
            {
                yield return new DropDownListModel
                {
                    Id = count,
                    Description = drive.Name
                };
                count++;
            }
        }
    }

    public static string[] GetLocalDriversFromMachine()
    {
        return Environment.GetLogicalDrives();
    }

    private static string FormatBytes(long bytes)
    {
        if (bytes >= 0x1000000000000000) { return ((double)(bytes >> 50) / 1024).ToString("0.### EB"); }
        if (bytes >= 0x4000000000000) { return ((double)(bytes >> 40) / 1024).ToString("0.### PB"); }
        if (bytes >= 0x10000000000) { return ((double)(bytes >> 30) / 1024).ToString("0.### TB"); }
        if (bytes >= 0x40000000) { return ((double)(bytes >> 20) / 1024).ToString("0.### GB"); }
        if (bytes >= 0x100000) { return ((double)(bytes >> 10) / 1024).ToString("0.### MB"); }
        if (bytes >= 0x400) { return ((double)bytes / 1024).ToString("0.###") + " KB"; }
        return bytes.ToString("0 Bytes");
    }
}
