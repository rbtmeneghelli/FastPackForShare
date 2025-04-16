using Azure.Storage.Files.Shares;
using FastPackForShare.Models;

namespace FastPackForShare.Extensions;

public static class AzureExtension
{
    /// <summary>
    /// Metodo respónsavel pelo download de um documento armazenado no file share do Azure
    /// </summary>
    /// <returns></returns>
    public static async Task<FileShareResponse> DownloadFile(FileShareRequest fileShareRequest)
    {
        ShareClient share = new ShareClient(fileShareRequest.ConnectionString, fileShareRequest.ShareReference);
        await share.CreateIfNotExistsAsync();

        if (await share.ExistsAsync())
        {
            ShareDirectoryClient directory = share.GetRootDirectoryClient();
            await directory.CreateIfNotExistsAsync();

            if (await directory.ExistsAsync())
            {
                ShareFileClient file = directory.GetFileClient(fileShareRequest.FileName);

                if (await file.ExistsAsync())
                {
                    //ShareFileDownloadInfo download = await file.DownloadAsync();
                    return new FileShareResponse(fileShareRequest.FileName, fileShareRequest.FileExtension, await file.OpenReadAsync());
                }
            }
        }

        return null;
    }
}