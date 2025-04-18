namespace FastPackForShare.Interfaces;

public interface IUserLoggedService
{
    public long UserId { get; }
    public string UserName { get; }
    public bool UserAuthenticated { get; }
}
