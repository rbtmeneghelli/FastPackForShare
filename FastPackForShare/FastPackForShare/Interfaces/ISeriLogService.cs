namespace FastPackForShare.Interfaces;

public interface ISeriLogService
{
    void WriteFatalLogger(string message);
    void WriteErrorLogger(string message);
    void WriteInformationLogger(string message);
}
