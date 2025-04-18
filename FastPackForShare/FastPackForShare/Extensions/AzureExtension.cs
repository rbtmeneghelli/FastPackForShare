using Azure;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Files.Shares;
using FastPackForShare.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.File;

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

    /// <summary>
    /// Metodo respónsavel pelo upload de um documento no storage/file shared do azure
    /// </summary>
    /// <param name="fileName">Nome do arquivo que será carregado no storage do Azure via upload</param>
    /// <returns></returns>
    public static async Task UploadFile(string azureConnectionStringStorage, string storageFolder, string fileName)
    {
        ShareClient shareClient = await GetStorageClientFileToUpload(azureConnectionStringStorage, storageFolder);
        ShareDirectoryClient shareDirectoryClient = shareClient.GetRootDirectoryClient();
        ShareFileClient shareFileClient = shareDirectoryClient.GetFileClient(fileName);

        using (FileStream stream = File.OpenRead(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName)))
        {
            if (await shareFileClient.ExistsAsync() == false)
            {
                await shareFileClient.CreateAsync(stream.Length);
            }

            await shareFileClient.UploadRangeAsync(new HttpRange(0, stream.Length), stream);
        }
    }

    public static async Task<string> GetSecretFromAzureKeyVault(string azureKeyVaultUrl, string secretName)
    {
        var client = new SecretClient(new Uri(azureKeyVaultUrl), new DefaultAzureCredential());
        KeyVaultSecret secret = await client.GetSecretAsync(secretName);
        return secret.Value;
    }

    #region Private Methods

    private static CloudFileClient GetStorageClientFileToDownload(string azureFileShareAccountName, string azureFileShareKeyValue)
    {
        var storageCredentials = new StorageCredentials(azureFileShareAccountName, azureFileShareKeyValue);
        CloudStorageAccount storageAccount = new CloudStorageAccount(storageCredentials, true);
        return storageAccount.CreateCloudFileClient();
    }

    private static async Task<ShareClient> GetStorageClientFileToUpload(string azureConnectionStringStorage, string storageFolder)
    {
        ShareServiceClient serviceClient = new ShareServiceClient(azureConnectionStringStorage);
        ShareClient shareClient = serviceClient.GetShareClient(storageFolder);

        if (!await shareClient.ExistsAsync())
        {
            await shareClient.CreateAsync();
        }

        return shareClient;
    }

    #endregion
}