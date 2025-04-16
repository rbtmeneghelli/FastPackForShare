namespace FastPackForShare.Interfaces;

public interface IDataFromApiService<T> where T : class
{
    Task<T> GetDataFromExternalAPI(string apiPath);
    Task<IEnumerable<T>> GetListFromExternalAPI(string apiPath);
    Task<bool> PostFromExternalAPI(string apiPath, T data);
    Task<bool> PutFromExternalAPI(string apiPath, T data);
}
