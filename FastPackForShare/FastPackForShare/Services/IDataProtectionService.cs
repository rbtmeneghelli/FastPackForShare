namespace FastPackForShare.Services;

public interface IDataProtectionService
{
    string ApplyProtect(string input);
    string RemoveProtect(string input);
}
