using System.Net.Http.Json;
using FastPackForShare.Constants;
using FastPackForShare.Interfaces;
using FastPackForShare.Services.Bases;

namespace FastPackForShare.Services;

public class DataFromApiService<T> : BaseHandlerService, IDataFromApiService<T> where T : class
{
    private readonly IHttpClientFactory _httpClientFactory;

    public DataFromApiService(IHttpClientFactory httpClientFactory, INotificationMessageService iNotificationMessageService) : base(iNotificationMessageService)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<T> GetDataFromExternalAPI(string apiPath)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("Signed");
            var response = await client.GetFromJsonAsync<T>(apiPath);
            return response;
        }
        catch (Exception ex)
        {
            Notify($"{ConstantValue.REQUEST_API} {apiPath}");
        }

        return null;
    }

    public async Task<IEnumerable<T>> GetListFromExternalAPI(string apiPath)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("Signed");
            var response = await client.GetFromJsonAsync<IEnumerable<T>>(apiPath);
            return response;
        }
        catch (Exception ex)
        {
            Notify($"{ConstantValue.REQUEST_API} {apiPath}");
        }

        return null;
    }

    public async Task<bool> PostFromExternalAPI(string apiPath, T data)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("Signed");
            var response = await client.PostAsJsonAsync(apiPath, data);
            response.EnsureSuccessStatusCode();
            return true;
        }
        catch (Exception ex)
        {
            Notify($"{ConstantValue.REQUEST_API} {apiPath}");
        }

        return false;
    }

    public async Task<bool> PutFromExternalAPI(string apiPath, T data)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("Signed");
            var response = await client.PutAsJsonAsync(apiPath, data);
            response.EnsureSuccessStatusCode();
            return true;
        }
        catch (Exception ex)
        {
            Notify($"{ConstantValue.REQUEST_API} {apiPath}");
        }

        return false;
    }
}
